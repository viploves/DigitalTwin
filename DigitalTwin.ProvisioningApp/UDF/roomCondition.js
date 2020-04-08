// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

var temperature = "Temperature";
var carbonDioxide = "CarbonDioxide";
var temperatureThreshold = 78;
var co2Threshold = 1000;
var dataType = "RoomCondition";
var unhealthyCondition = "High temperature or Carbon dioxide levels";
var healthyCondition = "Healthy conditions";

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

        var temperatureSensor = otherSensors.find(function(element) {
            return element.DataType === temperature;
        });

        var co2Sensor = otherSensors.find(function (element) {
            return element.DataType === carbonDioxide;
        });
        
        // get latest values for above sensors
        var temperatureValue = temperatureSensor.Value().Value;
        var co2Value = co2Sensor.Value().Value;

        // Return if no motion found return
        if (temperatureValue === null || co2Value == null) {
            sendNotification(telemetry.SensorId, "Sensor", "Error: Temperature or Carbon Dioxide is null, returning");
            return;
        }

        // log space computed value
        log(`Temperature: ${temperatureValue}. Carbon Dioxide: ${co2Value}`);

        if (temperatureValue > temperatureThreshold || co2Value > co2Threshold) {
            // Set parent space
            setSpaceValue(parentSpace.Id, dataType, unhealthyCondition);
        }
        else {
            // Set parent space
            setSpaceValue(parentSpace.Id, dataType, healthyCondition);
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
