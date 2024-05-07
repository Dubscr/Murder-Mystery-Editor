using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.Networking;
using System;

public class WorkshopHandler : MonoBehaviour
{
    [Serializable]
    public class UpdateItemParams
    {
        public string contentPath;
        public string imagePath;
        public string changeNotes;
    }

    public class WorkshopItemData
    {
        public string title;
        public ulong id;
    }

    public UpdateItemParams updateItemParams;
    public ulong currentlySelectedItemID;
    public List<WorkshopItemData> workshopItemData;

    private void Awake()
    {
        updateItemParams.contentPath = Application.dataPath + "/../AssetBundles";
        updateItemParams.imagePath = Application.dataPath + "/thumbnail.png";

        if (updateItemParams.imagePath.Contains("\\"))
        {
            updateItemParams.imagePath.Replace("\\", "/");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!SteamManager.Initialized) return;
        Debug.Log("Steam is initialized!");
        GetSteamWorkshopItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Create new workshop item
    /// </summary>
    public void CreateWorkshopItem()
    {
        // Retrieved from the project's steam_appid.txt. Can be manually inserted here as well
        var appId = SteamUtils.GetAppID();

        // Make the call to the steam back-end
        var createHandle = SteamUGC.CreateItem(appId, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        var callResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(HandleCreateItemResult));
        callResult.Set(createHandle);
    }
    /// <summary>
    /// Callback event for when the item creation event has completed.
    /// </summary>
    private void HandleCreateItemResult(CreateItemResult_t result, bool bIOFailure)
    {
        if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
        {
            var url = $"steam://url/CommunityFilePage/{result.m_nPublishedFileId}";
            Debug.LogError($"Cannot create workshop item. Please accept workshop legal agreement: {url}");
            Application.OpenURL(url);
        }

        if (result.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log($"Workshop item created! (https://steamcommunity.com/sharedfiles/filedetails/?id=" + result.m_nPublishedFileId + ")");
            Debug.Log("CODE: " + result.m_nPublishedFileId + "\n IT IS COPIED TO YOUR CLIPBOARD ALREADY");
            GUIUtility.systemCopyBuffer = result.m_nPublishedFileId.ToString();
        }
        else
            Debug.LogError($"Workshop item creation failed: {result.m_eResult}");
    }

    /// <summary>
    /// Retrieve all the user's subscribed workshop items
    /// </summary>
    private void GetSteamWorkshopItems()
    {
        Debug.Log("Getting steam workshop items...");

        // Retrieved from the project's `steam_appid.txt`. Can be manually inserted here as well
        var appId = SteamUtils.GetAppID();

        // Create a query request
        var queryRequest = SteamUGC.CreateQueryUserUGCRequest(
            SteamUser.GetSteamID().GetAccountID(),
            EUserUGCList.k_EUserUGCList_Subscribed,
            EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse,
            EUserUGCListSortOrder.k_EUserUGCListSortOrder_VoteScoreDesc,
            appId,
            appId,
            1);

        // Make the call to the steam back-end
        var queryHandle = SteamUGC.SendQueryUGCRequest(queryRequest);
        var callResult = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(HandleQueryCompleted));
        callResult.Set(queryHandle);
    }

    /// <summary>
    /// Handle the Steam UGC query result
    /// </summary>
    private void HandleQueryCompleted(SteamUGCQueryCompleted_t response, bool bIOFailure)
    {
        StartCoroutine(LoadItemsRoutine(response));
    }

    /// <summary>
    /// Coroutine to get the information from Steam Workshop items.
    /// </summary>
    private IEnumerator LoadItemsRoutine(SteamUGCQueryCompleted_t response)
    {
        for (uint i = 0; i < response.m_unNumResultsReturned; i++)
        {
            // Get the Steam Workshop item from the query
            SteamUGC.GetQueryUGCResult(response.m_handle, i, out var workshopItem);
            SteamUGC.DownloadItem(workshopItem.m_nPublishedFileId, true);
            // Get the size, folder and timestamp of the Steam Workshop item
            SteamUGC.GetItemInstallInfo(workshopItem.m_nPublishedFileId, out var size, out var contentFolder, 255, out var timestamp);

            Debug.Log(size);

            // Do something with the contents of the Steam Workshop item here!
            Debug.Log($"File content path: {contentFolder}");

            // Get the mod title
            Debug.Log(workshopItem.m_rgchTitle);

            // Get the mod description
            Debug.Log(workshopItem.m_rgchDescription);

            Debug.Log(workshopItem.m_nPublishedFileId);

            // Load the mod thumbnail image
            SteamUGC.GetQueryUGCPreviewURL(response.m_handle, i, out var imageUrl, 255);
            Sprite thumbnail = null;
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                    Debug.Log(uwr.error);
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    thumbnail = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
            }
        }
    }

    public void OnClickUpdateWorkshopItem()
    {
        UpdateWorkshopItem(new PublishedFileId_t(currentlySelectedItemID), updateItemParams);
    }

    /// <summary>
    /// Set or update a workshop item's content.
    /// </summary>
    public void UpdateWorkshopItem(PublishedFileId_t itemId, UpdateItemParams updateItemParams)
    {
        // Retrieved from the project's steam_appid.txt. Can be manually inserted here as well
        var appId = SteamUtils.GetAppID();

        // Initialize the item update
        var updateHandle = SteamUGC.StartItemUpdate(appId, itemId);

        // Sets the folder that will be stored as the content for an item. (https://partner.steamgames.com/doc/api/ISteamUGC#SetItemContent)
        SteamUGC.SetItemContent(updateHandle, updateItemParams.contentPath);

        if (!string.IsNullOrEmpty(updateItemParams.imagePath))
        {
            // Sets the primary preview image for the item. (https://partner.steamgames.com/doc/api/ISteamUGC#SetItemPreview)
            SteamUGC.SetItemPreview(updateHandle, updateItemParams.imagePath);
        }

        // Make the call to the steam back-end
        var itemUpdateHandle = SteamUGC.SubmitItemUpdate(updateHandle, updateItemParams.changeNotes);
        var callResult = CallResult<SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(HandleItemUpdateResult));
        callResult.Set(itemUpdateHandle);
    }

    /// <summary>
    /// Handle the item update result
    /// </summary>
    private void HandleItemUpdateResult(SubmitItemUpdateResult_t result, bool bIOFailure)
    {
        if (result.m_eResult == EResult.k_EResultOK)
        {
            var url = $"steam://url/CommunityFilePage/{result.m_nPublishedFileId}";
            Debug.Log($"Update Item success! ({url})");
        }
        else
            Debug.LogError($"Workshop item update failed: {result.m_eResult}");
    }


}
