using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LootLockerUtil
{
    public static IEnumerator EndWhiteLabelSession(Action<bool> OnComplete)
    {
        bool done = false;
        LootLockerSDKManager.EndSession(response =>
        {
            done = true;
            OnComplete?.Invoke(response.success);
        });
        yield return new WaitUntil(() => done);
    }

    public static IEnumerator GetPlayerName(Action<string> OnComplete)
    {
        bool done = false;
        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                OnComplete?.Invoke(response.name);
            }
            done = true;
        });
        yield return new WaitUntil(() => done);
    }

    public static IEnumerator ChangePlayerName(string newName, Action<bool> OnComplete)
    {
        bool done = false;
        LootLockerSDKManager.SetPlayerName(newName ,(response) =>
        {
            OnComplete?.Invoke(response.success);
            done = true;
        });
        yield return new WaitUntil(() => done);
    }

    public static IEnumerator SubmitToLeaderbaord(string memberID, int score, string leaderBoardID, string metaData, Action<bool> OnComplected)
    {
        bool isDone = false;
        LootLockerSDKManager.SubmitScore(memberID, score, leaderBoardID, metaData, (response) =>
        {
            isDone = true;
            OnComplected?.Invoke(response.success);
        });

        while (isDone == false) yield return null;
    }

    public static IEnumerator GetAllPlayerFiles(Action<LootLockerPlayerFile[]> result)
    {
        bool isDone = false;
        LootLockerSDKManager.GetAllPlayerFiles((response) =>
        {
            isDone = true;

            if (response.success)
            {
                result?.Invoke(response.items);
            }
            else
            {
                result?.Invoke(null);
            }
        });
        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator GetPlayerFile(int fileId, Action<LootLockerPlayerFile> result)
    {
        bool isDone = false;
        LootLockerSDKManager.GetPlayerFile(fileId, (response) =>
        {
            isDone = true;

            if (response.success)
            {
                result?.Invoke(response);
            }
            else
            {
                result?.Invoke(null);
            }
        });

        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator UploadPlayerFile(string filename, string filePurpose, byte[] fileBytes, bool isPublic, Action<bool, LootLockerPlayerFile> result)
    {
        bool isDone = false;

        LootLockerSDKManager.UploadPlayerFile(fileBytes, filename, filePurpose, isPublic, response =>
        {
            isDone = true;

            if (response.success)
            {
                result.Invoke(response.success, response);
            }
            else
            {
                result.Invoke(response.success,null);
            }
        });

        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator UpdatePlayerFile(int playerFileID, byte[] fileBytes, Action<bool, LootLockerPlayerFile> result)
    {
        bool isDone = false;

        LootLockerSDKManager.UpdatePlayerFile(playerFileID, fileBytes, response =>
        {
            isDone = true;

            if (response.success)
            {
                result.Invoke(response.success, response);
            }
            else
            {
                result.Invoke(response.success, null);
            }
        });

        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator DeletePlayerFile(int playerFileId, Action<bool> result)
    {
        bool isDone = false;

        LootLockerSDKManager.DeletePlayerFile(playerFileId, response =>
        {
            isDone = true;
            result.Invoke(response.success);
        });

        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator SaveToPlayerStorage(string key, string data, bool isPublic, Action<LootLockerPayload> result)
    {
        bool isDone = false;

        LootLockerSDKManager.UpdateOrCreateKeyValue(key, data, isPublic, (getPersistentStoragResponse) =>
        {
            isDone = true;
            result.Invoke(getPersistentStoragResponse.success ? getPersistentStoragResponse.payload[0] : null);
        });
        
        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator SaveMultipleToPlayerStroage(Dictionary<string, string> data, Action<bool, LootLockerPayload[]> result)
    {
        LootLockerGetPersistentStorageRequest lootData = new LootLockerGetPersistentStorageRequest();

        foreach (KeyValuePair<string,string> item in data)
        {
            lootData.AddToPayload(new LootLockerPayload { key = item.Key, value = item.Value });
        }

        bool isDone = false;

        LootLockerSDKManager.UpdateOrCreateKeyValue(lootData, (getPersistentStoragResponse) =>
        {
            isDone = true;
            result.Invoke(getPersistentStoragResponse.success, getPersistentStoragResponse.success ? getPersistentStoragResponse.payload: null);
        });

        yield return new WaitUntil(() => isDone);
    }

    public static IEnumerator LoadFromPlayerStroage(string fileName, Action<string> result)
    {
        bool isDone = false;

        LootLockerSDKManager.GetSingleKeyPersistentStorage(fileName, (response) =>
        {
            isDone = true;
            result.Invoke(response.success ? response.payload.value : null);
        });

        while (isDone == false) yield return null;
    }

    public static IEnumerator LoadAllPlayerStorage(Action<LootLockerPayload[]> result)
    {
        bool done = false;

        LootLockerSDKManager.GetEntirePersistentStorage((response) =>
        {
            done = true;
            result?.Invoke(response.success ? response.payload : null);
        });

        while (done == false) yield return null;
    }

    public static IEnumerator CreatePlayerProfile(string playerID, Action<LLPlayerProfile> OnComplete)
    {
        LLPlayerProfile player = new LLPlayerProfile(playerID);

        yield return GetPlayerName(name =>
        {
            player.name = name;
        });

        yield return LoadAllPlayerStorage(payloads =>
        {
            if (payloads != null)
            {
                foreach (LootLockerPayload payload in payloads)
                {
                    player.storage.Add(payload.key, payload.value);
                }
            }
        });

        yield return GetAllPlayerFiles(files =>
        {
            if (files != null)
            {
                player.files = files.ToList();
            }
        });

        OnComplete?.Invoke(player);
    }

    public static IEnumerator DeleteKeyData(string keyToDelete, Action OnCompleted)
    {
        bool done = false;

        LootLockerSDKManager.DeleteKeyValue(keyToDelete ,(response) =>
        {
            done = true;
            OnCompleted?.Invoke();
        });

        while (done == false) yield return null;
    }
}
