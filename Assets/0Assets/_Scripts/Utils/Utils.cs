using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Utils
{
    /// <summary>
    /// Of all the gameobjects saved in contactList parameter, take and return the nearest to player.
    /// </summary>
    /// <returns></returns>
    public static GameObject GetNearestObject(Transform transform, List<GameObject> contactList)
    {
        GameObject nearestGameObject = null;
        float minDistance = float.MaxValue;
        float distance;

        foreach (GameObject contactObject in contactList)
        {
            distance = (contactObject.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance && contactObject.gameObject.activeInHierarchy)
            {
                minDistance = distance;
                nearestGameObject = contactObject;
            }
        }

        return nearestGameObject;
    }

    /// <summary>
    /// Of all the rigidbodies saved in contactList parameter, take and return the nearest to player.
    /// </summary>
    /// <returns></returns>
    public static Rigidbody GetNearestObject(Transform transform, List<Rigidbody> contactList)
    {
        Rigidbody nearestRigidBody = null;
        float minDistance = float.MaxValue;
        float distance;

        foreach (Rigidbody contactBody in contactList)
        {
            distance = (contactBody.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance && contactBody.gameObject.activeInHierarchy)
            {
                minDistance = distance;
                nearestRigidBody = contactBody;
            }
        }

        return nearestRigidBody;
    }

    /// <summary>
    /// Of all the transforms saved in contactList parameter, take and return the nearest to player.
    /// </summary>
    /// <returns></returns>
    public static Transform GetNearestObject(Transform transform, List<Transform> contactList)
    {
        Transform nearestTransform = null;
        float minDistance = float.MaxValue;
        float distance;

        foreach (Transform contactTransform in contactList)
        {
            distance = (contactTransform.position - transform.position).sqrMagnitude;

            if (distance < minDistance && contactTransform.gameObject.activeInHierarchy)
            {
                minDistance = distance;
                nearestTransform = contactTransform;
            }
        }

        return nearestTransform;
    }

    // Download Files
    public static T ParseJSON<T>(string json)
    {
        T ret = JsonUtility.FromJson<T>(json);
        return ret;
    }

    public static IEnumerator RequestWebService(string url, System.Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
            request.useHttpContinue = false;
            request.certificateHandler = new AcceptAllCertificates();

            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError($"Error getting online file ({url}): {request.error}");
            }
            else
            {
                Debug.Log($"File from {url} readed succesfully");

                var text = request.downloadHandler.text;
                callback(text);
            }
        }
    }

    // Environment
    public static IDictionary ReadEnvironmentVariables()
    {
        
        try
        {
            var ret = Environment.GetEnvironmentVariables();
            if (ret.Count > 0)
                return ret;
            else
            {
                Debug.LogWarning("Don't have Environment variables !");
                return null;
            }
        }
        catch
        {
            return null;
        }
    }
    public static bool? ReadBoolEnvironment(string envName)
    {
        try
        {
            string bool_env = Environment.GetEnvironmentVariable(envName);
            if (bool_env != null && bool_env.ToLower() == "true") return true;
            else if (bool_env != null && bool_env.ToLower() == "false") return false;
            else return null;

        }
        catch
        {
            return null;
        }
    }
    public static string ReadStringEnvironment(string envName)
    {
        try
        {
            var tmpValue = "";
            tmpValue = Environment.GetEnvironmentVariable(envName);
            return (tmpValue != null && !string.IsNullOrWhiteSpace(tmpValue)) ? tmpValue : null;
        }
        catch
        {
            return null;
        }
    }
    public static int? ReadIntEnvironment(string envName)
    {
        int ret;
        try
        {
            var tmpValue = "";
            tmpValue = Environment.GetEnvironmentVariable(envName);
            if (tmpValue != null && Int32.TryParse(tmpValue, out ret))
                return ret;
            else
                return null;
        }
        catch
        {
            return null;
        }
    }
    public static float? ReadFloatEnvironment(string envName)
    {
        float ret;
        try
        {
            var tmpValue = "";
            tmpValue = Environment.GetEnvironmentVariable(envName);
            if (tmpValue != null && float.TryParse(tmpValue, out ret))
                return ret;
            else
                return null;
        }
        catch
        {
            return null;
        }
    }
    public static bool? ReadProtocolEnvironment(string envName)
    {
        try
        {
            var tmpValue = Environment.GetEnvironmentVariable(envName);
            Debug.Log($"{envName} values is {tmpValue}");
            if (tmpValue != null) return (tmpValue.ToLower() == "https");
            else return null;
        }
        catch
        {
            return null;
        }
    }
}

class AcceptAllCertificates : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}