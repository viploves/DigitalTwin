// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

var motionType = "Motion";
var dataType = "RoomAvailability";
var roomAvailable = "Room Available";
var roomUnavailable = "Room Unavailable";

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

        var motionSensor = otherSensors.find(function(element) {
            return element.DataType === motionType;
        });
        
        // get latest values for above sensors
        var motionValue = motionSensor.Value().Value;

        // Return if no motion found return
        if (motionValue === null) {
            sendNotification(telemetry.SensorId, "Sensor", "Error: Motion is null, returning");
            return;
        }

        // log and set parent space computed value
        log(`Presence: ${!motionValue}.`);

        if (!motionValue) {
            // Set parent space
            setSpaceValue(parentSpace.Id, dataType, roomAvailable);
        }
        else {
            // Set parent space
            setSpaceValue(parentSpace.Id, dataType, roomUnavailable);
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
