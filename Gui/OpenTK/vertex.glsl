#version 130

in vec3 vertex_m;
in vec3 norm_m;
in float height;

uniform float maxheight;
uniform vec3 lightPos_w;
uniform mat4 P;
uniform mat4 M;
uniform mat4 V;


varying vec3 position_w;
varying vec3 normal_c;
varying vec3 eyeDirection_c;
varying vec3 lightDirection_c;
varying float oheight;
varying vec3 vertex_c;

void main() {

  //float foo = height / maxheight;
  vec3 vertex_a_m = vertex_m * height / 6378.1;
  oheight = height;
  
  mat4 MVP = P * V * M;
  gl_Position = MVP * vec4(vertex_a_m, 1.0);
  position_w = (M * vec4(vertex_a_m, 1.0)).xyz;
  
  vertex_c = (V * M * vec4(vertex_a_m, 1)).xyz;
  eyeDirection_c = -vertex_c;

  vec3 lightPos_c = (V * vec4(lightPos_w, 1)).xyz;
  lightDirection_c = eyeDirection_c + lightPos_c;

  normal_c = (V * M * vec4(normalize(norm_m), 0)).xyz;
  //normal_c = normal_m;
  //normal_c = (V * M * vec4(gl_Normal, 0)).xyz;
}
