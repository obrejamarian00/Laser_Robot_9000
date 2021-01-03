#include<Servo.h>
Servo serX;
Servo serY;
String serialData;

void setup() {
  serX.attach(2);
  serY.attach(3);
  Serial.begin(9600);
  Serial.setTimeout(10);
  
}

void loop() {
  //
}

void serialEvent(){
  serialData = Serial.readString();

  serX.write(parseDataX(serialData));
  serY.write(parseDataY(serialData));
}

int parseDataX(String data){
  data.remove(data.indexOf("Y"));
  data.remove(data.indexOf("X"), 1);
  int writeX = data.toInt();
  writeX = map(writeX, 0, 640, 128, 56);
  return writeX;
}
int parseDataY(String data){
  data.remove(0, data.indexOf("Y") +1);
  int writeY = data.toInt();
  writeY = map(writeY, 0, 480, 62, 118);
  return writeY;
}
