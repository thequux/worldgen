#version 130

varying vec3 position_w;
varying vec3 normal_c;
varying vec3 eyeDirection_c;
varying vec3 vertex_c;
varying vec3 lightDirection_c;
varying float oheight;

uniform mat4 M;
uniform mat4 V;
uniform vec3 lightPos_w;

uniform float maxheight;

uniform int errp;

out vec3 color;

void main() {

  float waterlevel = 6378.1;
  vec3 lightColor = vec3(1,1,1);
  float lightPower = 50.0f;

  bool drawLine = fract((oheight - waterlevel) / 60) < 0.05;
  bool underwater = oheight < waterlevel;
  vec3 materialDiffuseColor = underwater
    ? (drawLine ? vec3(0,1,0) : vec3(0,0,1))
    : (drawLine ? vec3(1,0,0) : vec3(0.8, 0.8, 0.8));
     
  
  vec3 materialAmbientColor = materialDiffuseColor / 5;
  vec3 materialSpecularColor = vec3(0.1, 0.1, 0.1) * 5;
  
  float distance = length(lightPos_w - position_w);
  vec3 n = normalize(normal_c);
  vec3 l = normalize(lightDirection_c);

  float cosTheta = clamp(dot(n,l), 0, 1);
  vec3 E = normalize(eyeDirection_c);
  vec3 R = reflect(-l, n);

  float cosAlpha = clamp(dot(E,R), 0, 1);
  //float fd = distance < 0.993 ? 1 : 0;
  //color = vec3(fd,0,0);
  color.rgb = materialAmbientColor +
    materialDiffuseColor * lightColor * lightPower * cosTheta / pow(distance, 2) +
    materialSpecularColor * lightColor * lightPower * cosAlpha / pow(distance, 2);

  //color.rgb = 0.5-normal_c;
  // debugging aid: invert colors on error.
  if (errp != 0) {
    color.rgb = 1 - color.rgb;
  }
}
