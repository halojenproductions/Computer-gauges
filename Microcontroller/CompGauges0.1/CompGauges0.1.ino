//#include <SoftwareSerial.h>

//SoftwareSerial mySerial(10, 11); // RX, TX

int _out = 5;
int incomingByte = 0;
String inString = "";    // string to hold input
int dialval = 0;

void setup() {
  // put your setup code here, to run once:
  pinMode(_out, OUTPUT);
  Serial.begin(115200);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }
  dialval = map(100,0,100,0,255);
  analogWrite(_out, dialval);
}

void loop() {
  // put your main code here, to run repeatedly:
  while (Serial.available()>0) {
    int inChar = Serial.read();
    if (isDigit(inChar)) {
      // convert the incoming byte to a char and add it to the string:
      inString += (char)inChar;
    }
    if (inChar == '\n') {
      // if you get a newline, print the string, then the string's value
      Serial.print("Value:");
      Serial.println(dialval);
      Serial.print("String: ");
      Serial.println(inString);

      dialval = map(inString.toInt(),0,100,0,255);
      analogWrite(_out, dialval);

      inString = "";      // clear the string for new input.
    }
 
    
     /*incomingByte = Serial.read().toInt();
     //val = map(val,0,100,0,254);
     analogWrite(_out, incomingByte);
     Serial.write(incomingByte);*/
  
 }


  return;
  for (int i=0;i<=254;i++){
    analogWrite(_out, i);
    delay(20);
  }
  for (int i=254;i>=0;i--){
    analogWrite(_out, i);
    delay(20);
  }
  return;
  analogWrite(_out, 63);
  delay(1000);
  analogWrite(_out, 127);
  delay(1000);
  analogWrite(_out, 190);
  delay(1000);
  analogWrite(_out, 254);
  delay(1000);
  analogWrite(_out, 0);
  delay(1000);
  analogWrite(_out, 254);
  delay(1000);
  analogWrite(_out, 127);
  delay(1000);
  analogWrite(_out, 254);
  delay(1000);
}
