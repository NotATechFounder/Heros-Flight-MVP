using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LLPlayerProfile
{
    public string id;
    public string name;
    public Dictionary<string, string> storage = new Dictionary<string, string>();
    public List<LootLockerPlayerFile> files = new List<LootLockerPlayerFile>();

    public LLPlayerProfile(string id, string name = "UnKnown")
    {
        this.id = id;
        this.name = name;
    }

    public IEnumerator UploadOrUpdateWebFile(string fileName, string purpose, byte[] bytes, bool isPublic)
    {
        if (files == null) files = new List<LootLockerPlayerFile>();

        LootLockerPlayerFile lockerPlayerFile = files.FirstOrDefault(file => file.name == fileName);

        if (lockerPlayerFile == null)
        {
            yield return LootLockerUtil.UploadPlayerFile(fileName, purpose, bytes, isPublic, (result, savedFile) =>
            {
                files.Add(savedFile);
                Debug.Log("Upload - Publish :" + result);
            });
        }
        else
        {
            yield return LootLockerUtil.UpdatePlayerFile(GetWebFileID(fileName), bytes, (result, savedFile) =>
            {
                files[files.FindIndex(file => file.name == fileName)] = savedFile;
                Debug.Log("Update - Publish :" + result);
            });
        }
    }

    public IEnumerator RemoveWebFie(string fileName, Action<bool> Oncomplected)
    {
        if (files == null || files.Count == 0)
        {
            Debug.Log("Remove WebFie Failed :File not found");
            Oncomplected?.Invoke(true);
            yield break;
        }

        LootLockerPlayerFile webFile = files.Find(file => file.name == fileName);
        if (webFile != null)
        {
            yield return LootLockerUtil.DeletePlayerFile(webFile.id, result =>
            {
                if (result) files.Remove(webFile);
                Oncomplected?.Invoke(result);
                Debug.Log("Deleted last file saved :" + result);
            });
        }
        else
        {
            Debug.Log("Remove WebFie Failed :File not found");
            Oncomplected?.Invoke(true);
        }
    }

    public string GetWebFileUrl(string fileName)
    {
        if (files == null || files.Count == 0) return "";
        LootLockerPlayerFile webFile = files.Find(file => file.name == fileName);
        return webFile == null ? "" : webFile.url;
    }

    public int GetWebFileID(string fileName)
    {
        if (files == null || files.Count == 0) return -1;
        LootLockerPlayerFile webFile = files.Find(file => file.name == fileName);
        return webFile == null ? -1 : webFile.id;
    }

    public LootLockerPlayerFile GetWebFile(string fileName)
    {
        if (files == null || files.Count == 0) return null;
        LootLockerPlayerFile webFile = files.Find(file => file.name == fileName);
        return webFile;
    }

    public bool HasWebFile(string fileName)
    {
        if (files == null)
        {
            Debug.Log("Player files is null :" + id);
            return false;
        }

        if (files.Count == 0)
        {
            Debug.Log("Player has no files : " + id);
            return false;
        }

        Debug.Log(id + " has files:" + files.Count);
        return files.Any(file => file.name == fileName);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        LLPlayerProfile player = (LLPlayerProfile)obj;
        return id == player.id;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}