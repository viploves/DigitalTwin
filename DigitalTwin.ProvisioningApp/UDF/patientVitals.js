// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

var bloodPressureType = "BloodPressure";
var bodyTemperature = "BodyTemperature";
var spaceHaleHearty = "HaleAndHearty";

var bloodPressureThreshold = 150;
var bodyTemperatureThreshold = 99;

function process(telemetry, executionContext) {

    try {
        // Log SensorId and Message
        log(`Sensor ID: ${telemetry.SensorId}. `);
        log(`Sensor value: ${JSON.stringify(telemetry.Message)}.`);

        // Get sensor metadata
        var sensor = getSensorMetadata(telemetry.SensorId);

        // Retrieve the sensor reading
        var parseReading = JSON.parse(telemetry.Message);

        // Set the sensor reading as the current value for the sensor.
        setSensorValue(telemetry.SensorId, sensor.DataType, parseReading.SensorValue);

        // Get parent space
        var parentSpace = sensor.Space();

        // Get children sensors from the same space
        var otherSensors = parentSpace.ChildSensors();

        // Retrieve carbonDioxide, and motion sensors
        var carbonDioxideSensor = otherSensors.find(function(element) {
            return element.DataType === bloodPressureType;
        });

        var bodybloodPressureSensor = otherSensors.find(function(element) {
            return element.DataType === bodyTemperature;
        });
        
        // Add your sensor variable here
        var bloodPressureSensor = otherSensors.find(function (element) {
            return element.DataType === temperatureType;
        });

        // get latest values for above sensors
        var bodyTemperatureValue = bodybloodPressureSensor.Value().Value;
        var bloodPressureValue = getFloatValue(bloodPressureSensor.Value().Value);

        if (bodyTemperatureValue === null || bloodPressureValue === null) {
            sendNotification(telemetry.SensorId, "Sensor", "Error: Blood Pressure or Body Temperature are null, returning");
            return;
        }

        var alert = "The patient is hale and hearty";
        var noAlert = "The patient is sick!";

        if (bodyTemperatureValue < bodyTemperatureThreshold && temperatureValue < bloodPressureThreshold) {
            log(`${alert}. Body Temperature: ${bodyTemperatureValue}. Blood Pressure: ${bloodPressureValue}.`);

            // log, notify and set parent space computed value
            setSpaceValue(parentSpace.Id, spaceHaleHearty, alert);

            // Set up notification for this alert
            parentSpace.Notify(JSON.stringify(alert));
        }
        else {
            log(`${alert}. Body Temperature: ${bodyTemperatureValue}. Blood Pressure: ${bloodPressureValue}.`);

            // log, notify and set parent space computed value
            setSpaceValue(parentSpace.Id, spaceHaleHearty, noAlert);
        }
    }
    catch (error)
    {
        log(`An error has occurred processing the UDF Error: ${error.name} Message ${error.message}.`);
    }
}

function getFloatValue(str) {
  if(!str) {
      return null;
  }

  return parseFloat(str);
}
