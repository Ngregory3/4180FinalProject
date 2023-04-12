#include<sys/socket.h>
#include<arpa/inet.h>
#include<unistd.h>
#include<pthread.h>
#include<stdio.h>
#include<string.h>
#include<stdlib.h>
#include<fcntl.h>
#include<errno.h>
#include<termios.h>

void *passThrough(void *);
int fd;
volatile int numSocks;

int main(int argc , char *argv[]) {
	fd = open("/dev/ttyACM0", O_RDWR | O_NOCTTY | O_NDELAY);
	if (fd == -1) {
		perror("Unable to open /dev/ttyACM0");
		return (1);
	}
	fcntl(fd, F_SETFL, O_NONBLOCK);
	
	struct termios options;
	tcgetattr(fd, &options);
	cfsetspeed(&options, B9600);
	options.c_cflag &= ~CSTOPB;
	options.c_cflag |= CLOCAL;
	options.c_cflag |= CREAD;
	cfmakeraw(&options);
	tcsetattr(fd, TCSANOW, &options);
	sleep(1);
	
	int serverSocket, clientSocket, addrsize;
	struct sockaddr_in server, client;

	serverSocket = socket(AF_INET, SOCK_STREAM, 0);
	if (serverSocket == -1) printf("Could not create socket");
	
	server.sin_family = AF_INET;
	server.sin_addr.s_addr = INADDR_ANY;
	server.sin_port = htons(61000);
	
	if( bind(serverSocket,(struct sockaddr *)&server , sizeof(server)) < 0){
		puts("bind failed");
		return 1;
	}
	puts("bind done");
	
	listen(serverSocket, 3);
	
	puts("Waiting for connection request...");
	addrsize = sizeof(struct sockaddr_in);
	while (1) {
		clientSocket = accept(serverSocket, (struct sockaddr *)&client, (socklen_t*)&addrsize);
		puts("Connection accepted...");
		numSocks++;
		
		pthread_t connection;
		
		if(pthread_create(&connection, NULL, passThrough , &clientSocket) < 0) {
			perror("Error creating thread");
			return 1;
		}
	}
	
	return 0;
}

void* passThrough(void* socket) {
	int newsock = *(int*)socket;
	int sockID = numSocks;
	
	fcntl(newsock, F_SETFL, O_NONBLOCK);
	char pcbuffer[4];
	char mbedbuffer[4];
	
	
	while (1) {
		if (sockID < numSocks) {
			return 0;
		}
		
		if (recv(newsock, pcbuffer, 4, 0) > 0) {
			write(fd, pcbuffer, 4);
		}
		if (read(fd, mbedbuffer, 4) > 0) {
			send(newsock, mbedbuffer, 4, MSG_NOSIGNAL);
		}
	}
}