// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DigitalTwin.ProvisioningApp
{
    public partial class Api
    {
        public static async Task<IEnumerable<Models.Ontology>> GetOntologies(
            HttpClient httpClient,
            ILogger logger)
        {
            var response = await httpClient.GetAsync($"ontologies");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ontologies = JsonConvert.DeserializeObject<IEnumerable<Models.Ontology>>(content);
                logger.LogInformation($"Retrieved Ontologies: {JsonConvert.SerializeObject(ontologies, Formatting.Indented)}");
                return ontologies;
            }
            else
            {
                return Array.Empty<Models.Ontology>();
            }
        }

        public static async Task<Models.Resource> GetResource(
            HttpClient httpClient,
            ILogger logger,
            Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetResource requires a non empty guid as id");

            var response = await httpClient.GetAsync($"resources/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var resource = JsonConvert.DeserializeObject<Models.Resource>(content);
                logger.LogInformation($"Retrieved Resource: {JsonConvert.SerializeObject(resource, Formatting.Indented)}");
                return resource;
            }

            return null;
        }

        public static async Task<Models.Space> GetSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid id,
            string includes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetSpace requires a non empty guid as id");

            var response = await httpClient.GetAsync($"spaces/{id}/" + (includes != null ? $"?includes={includes}" : ""));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var space = JsonConvert.DeserializeObject<Models.Space>(content);
                logger.LogInformation($"Retrieved Space: {JsonConvert.SerializeObject(space, Formatting.Indented)}");
                return space;
            }

            return null;
        }

        public static async Task<Models.device> GetDevice(
            HttpClient httpClient,
            ILogger logger,
            Guid id,
            string includes = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("GetDevice requires a non empty guid as id");

            var response = await httpClient.GetAsync($"devices/{id}/" + (includes != null ? $"?includes={includes}" : ""));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var device = JsonConvert.DeserializeObject<Models.device>(content);
                logger.LogInformation($"Retrieved Device: {JsonConvert.SerializeObject(device, Formatting.Indented)}");
                return device;
            }

            return null;
        }

        public static async Task<IEnumerable<Models.Space>> GetSpaces(
            HttpClient httpClient,
            ILogger logger,
            int maxNumberToGet = 10,
            string includes = null,
            string propertyKey = null)
        {
            var includesFilter = (includes != null ? $"includes={includes}" : "");
            var propertyKeyFilter = (propertyKey != null ? $"propertyKey={propertyKey}" : "");
            var topFilter = $"$top={maxNumberToGet}";
            var response = await httpClient.GetAsync($"spaces{MakeQueryParams(new [] {includesFilter, propertyKeyFilter, topFilter})}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var spaces = JsonConvert.DeserializeObject<IEnumerable<Models.Space>>(content);
                logger.LogInformation($"Retrieved {spaces.Count()} Spaces");
                return spaces;
            }
            else
            {
                return Array.Empty<Models.Space>();
            }
        }

        public static async Task<IEnumerable<Models.Sensor>> GetSensorsOfSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid spaceId)
        {
            var devices = await GetDevicesOfSpace(httpClient, logger, spaceId);
            var allSensors = new List<Models.Sensor>();

            foreach (var device in devices)
            {
                var response = await httpClient.GetAsync($"sensors?deviceId={device.Id.ToString()}&includes=Types");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sensors = JsonConvert.DeserializeObject<IEnumerable<Models.Sensor>>(content);
                    allSensors.AddRange(sensors);
                }
            }

            return allSensors;
        }

        public static async Task<IEnumerable<Models.device>> GetDevicesOfSpace(
            HttpClient httpClient,
            ILogger logger,
            Guid spaceId)
        {
            var response = await httpClient.GetAsync($"devices?spaceId={spaceId.ToString()}&includes=Types");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonConvert.DeserializeObject<IEnumerable<Models.device>>(content);
                logger.LogInformation($"Retrieved {devices.Count()} Devices");
                return devices;
            }
            else
            {
                return Array.Empty<Models.device>();
            }
        }

        private static string MakeQueryParams(IEnumerable<string> queryParams)
        {
            return queryParams
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select((s, i) => (i == 0 ? '?' : '&') + s)
                .Aggregate((result, cur) => result + cur);
        }
    }
}