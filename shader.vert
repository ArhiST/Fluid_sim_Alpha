#version 330 core

layout(location = 0) in vec4 aPosition;  


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 translation;


void main(void)
{	
    gl_Position =  aPosition * translation * model * view * projection ; 
	
}