#version 130

varying vec3 position_w;
varying vec3 normal_c;
varying vec3 eyeDirection_c;
varying vec3 lightDirection_c;
varying float oheight;

uniform mat4 M;
uniform mat4 V;
uniform vec3 lightPos_w;

out vec3 color;

void main() {
  vec3 lightColor = vec3(1,1,1);
  float lightPower = 40.0f;

  vec3 materialDiffuseColor = fract(oheight * 40) < 0.03 ? vec3(0,0,1) : vec3(0.8, 0.8, 0.8);
  vec3 materialAmbientColor = materialDiffuseColor / 5;
  vec3 materialSpecularColor = vec3(0.1, 0.1, 0.1) / 5;
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
    materialSpecularColor * lightColor * lightPower * pow(cosAlpha, 5) / pow(distance, 2);
}
