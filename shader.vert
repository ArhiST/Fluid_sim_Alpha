#version 330 core

layout(location = 0) in vec4 aPosition;  


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;



void main(void)
{	
    gl_Position = aPosition * model * view * projection; 
	
}