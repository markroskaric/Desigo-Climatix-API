# DesigoClimatixApi

A C# library (.NET Standard 2.0) designed for communication with **Siemens Desigo CC** (Climatix) controllers with their JSON API interface. This library simplifies reading and writing point values using Base64 identifiers.

## 📥 Installation

Currently, you can include this library in your project by adding a reference to the `DesigoClimatixApi.dll` found in the [Releases](https://github.com/markroskaric/Desigo-Climatix-API/releases) tab.

## 📂 Desigo CC Integration

To use this library within Desigo CC scripts:

1. Copy `DesigoClimatixApi.dll` to the following folder: `\GMSMainProject\scriptingLibraries` in Desigo CC project

2. In your Desigo CC script, import the namespace and initialize the connection:

```javascript
// Import the namespace from your DLL
var climatix = importNamespace("DesigoClimatixApi");

// Initialize the connection
// Note: Ensure the DLL has a public constructor available
// Initialize the connection
var con = new climatix.Connection(
  "admin", // username
  "password123", // password
  "http://10.201.180.16", // controller IP/URL
  "7659" // PIN code
);

// Read value
// Parameter: (base64Id)
var resultRead = con.ReadValue("AiN4e05FAAE=");
console("Current Value: " + resultRead);

// Write value
// Parameters: (base64Id, value)
// "AiN4e05FAAE=" is the Point ID
// "1" is the new value you want to set
var resultWrite = con.WriteValue("AiN4e05FAAE=", "1");
console("Write Result: " + resultWrite);
```

# Data Handling & Formatting

This library is designed to simplify Siemens Climatix API responses. It automatically strips away complex JSON structures and returns only the clean, essential value needed for Desigo CC.

### How it works:

| Raw JSON Response from Controller (`Content`) | Clean Output Value |
| :-------------------------------------------- | :----------------- |
| `{"values":{"ID":".IO.P.AI.TZS.Val"}}`        | `.IO.P.AI.TZS.Val` |
| `{"values":{"ID":"°C"}}`                      | `°C`               |
| `{"values":{"ID":[5, 5]}}`                    | `5`                |
| `{"values":{"ID":[100, 100]}}`                | `100`              |
| `{"values":{"ID":"100"}}`                     | `100`              |

## Write Operations

When sending data to the controller, the library simplifies the response into a clear status message:

- **`Success`**: The value was successfully updated.
- **`Write Failed`**: The update failed (check the ID, credentials, or network connection).

## 🔧 Advanced Features & Debugging

If you need the full raw data instead of just the cleaned value, you can enable **Developer Mode** (`devMode = true`).

### **The Developer Object**

When `devMode` is active, the library returns a complete object containing all response details. This is useful for troubleshooting connection issues or seeing the raw JSON structure.

**Object Structure:**

- **`IsSuccess`**: Boolean (True if the request reached the controller).
- **`StatusCode`**: The raw HTTP status code (e.g., 200, 401, 404).
- **`Content`**: The raw, unformatted JSON string from the controller.
- **`ErrorMessage`**: Details of any internal library or network errors.
- **`PointId`**: The Base64 ID used in the request.
- **`APICall`**: The full URL used for the request.
- **`Op`**: The operation type (`Read` or `Write`).

### **How to use it?**

Use this mode if you need to debug why a value is "Not Found" or if you need to log the exact `APICall` being sent to the Siemens Climatix hardware.

```javascript
// Import the namespace from your DLL
var climatix = importNamespace("DesigoClimatixApi");

// Initialize the connection
// Note: Ensure the DLL has a public constructor available
// Initialize the connection
var con = new climatix.Connection(
  "admin", // username
  "password123", // password
  "http://10.201.180.16", // controller IP/URL
  "7659", // PIN code
  true //dev mode default false
);

// Read value
// Parameter: (base64Id)
var resultRead = con.ReadValue("AiN4e05FAAE=");
console("Current Value: " + resultRead);

// Write value
// Parameters: (base64Id, value)
// "AiN4e05FAAE=" is the Point ID
// "1" is the new value you want to set
var resultWrite = con.WriteValue("AiN4e05FAAE=", "1");
console("Write Result: " + resultWrite);
```
