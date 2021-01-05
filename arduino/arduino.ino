#include <ArduinoJson.h>

int tempPin = 1;
int pressurePin = 3;
unsigned long startMillis;  
unsigned long currentMillis;
const unsigned long period = 1000;  
const signed long offset = -3.1;
 
void setup(){
  Serial.begin(9600);
}
 
void loop(){

  currentMillis = millis();
  if (currentMillis - startMillis >= period){

      int tempReading = analogRead(tempPin);

      float voltage = tempReading * (5.0 / 1024.0);
      float temperatureC = (voltage - 0.5) * 100.0 ;  
      temperatureC += offset;

      int pressureReading = analogRead(pressurePin);
      float pressureVoltage = pressureReading * (5.0 / 1024.0);
      float pressureKPa = ((pressureVoltage / 5.0) + 0.04) * 1 / 0.004;

      DynamicJsonBuffer jsonBuffer;
      JsonObject& object = jsonBuffer.createObject();
      object["TemperatureInDegreesC"] = temperatureC;
      object["PressureInKPa"] = pressureKPa;
      
      object.printTo(Serial);
      Serial.println();
      startMillis = currentMillis;
  }
}
