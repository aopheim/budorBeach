#include <ArduinoJson.h>

int tempPin = 1;
int pressurePin = 3;
 
void setup(){
  Serial.begin(9600);
}
 
void loop(){
    int tempReading = analogRead(tempPin);

     // converting that reading to voltage, for 3.3v arduino use 3.3
    float voltage = tempReading * 5.0;
    voltage /= 1024.0; 

    float temperatureC = (voltage - 0.5) * 100 ;  //converting from 10 mv per degree wit 500 mV offset
                                               //to degrees ((voltage - 500mV) times 100)

    int pressureReading = analogRead(pressurePin);
    float pressureVoltage = pressureReading * (5.0 / 1024.0);
    float pressureKPa = ((pressureVoltage / 5.0) + 0.04) * 1 / 0.004;
    

    DynamicJsonBuffer jsonBuffer;
    JsonObject& object = jsonBuffer.createObject();
    object["TemperatureInDegreesC"] = temperatureC;
    object["PressureInKPa"] = pressureKPa;
    
    object.printTo(Serial);
    Serial.println();
    delay(5000);
}
