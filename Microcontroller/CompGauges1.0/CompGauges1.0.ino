#include <SoftwareSerial.h>

//SoftwareSerial mySerial(10, 11); // RX, TX
// The serial in pin is D2. Hence tie it to D0.

// Only 6 PWM outputs on the Duemelanove/AtMega328. These are them.
const int _outs[]= { 3,5,6,9,10,11};
const int _smooth = 12;
const int _diag = 13;

int incomingByte = 0;
String sInValue = ""; // string to hold input
int iCounterOffset = 0;

int iDialVals[6][2]={};
int iSmooth = 0;

void setup()
{
	pinMode(_smooth, INPUT_PULLUP);

	// Set up the gauge output pins.
	for (int i = 0;i<6;i++){
		pinMode(_outs[i], OUTPUT);
	}
	Serial.begin(115200);
	while (!Serial)
	{
		; // wait for serial port to connect. Needed for native USB port only.
	}
	analogWrite(_outs[1], map(100, 0, 100, 0, 255));
}
void DoOutputs(){
	for (int i = 0;i<6;i++){
		int iVal=iDialVals[i][iSmooth];
		analogWrite(_outs[i], map(iVal,0, 100, 0, 255));
	}
}
void loop(){
	iSmooth=!digitalRead(_smooth);
	digitalWrite(_diag,iSmooth);

	while (Serial.available() > 0){
		int inChar = Serial.read();
		if (isDigit(inChar)){
			// convert the incoming byte to a char and add it to the string:
			sInValue += (char)inChar;
		} else if (inChar == ':') {
			// Which gauge is the value for?
			iCounterOffset = sInValue.toInt();
			sInValue = "";
		}else if (inChar == ','){
			// Short average.
			iDialVals[iCounterOffset][0] = sInValue.toInt();
			sInValue = "";
		}else if (inChar == ';'){
			// Long average.
			iDialVals[iCounterOffset][1] = sInValue.toInt();
			sInValue = "";
		}else if (inChar == '\n'){
			/*Serial.print("Value:");
			Serial.println(dialval);
			Serial.print("String: ");
			Serial.println(inString);*/
			DoOutputs();

			sInValue = ""; // clear the string for new input.
		}

		/*incomingByte = Serial.read().toInt();
     //val = map(val,0,100,0,254);
     analogWrite(_out, incomingByte);
     Serial.write(incomingByte);*/
	 
	}

	/*return;
	for (int i = 0; i <= 254; i++)
	{
		analogWrite(_out, i);
		delay(20);
	}
	for (int i = 254; i >= 0; i--)
	{
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
	delay(1000);*/
}
