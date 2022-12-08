using RDTools;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SteamWorkshop
{
	public enum WorkshopError
	{
		None = -1,
		TagsConversion = 0,
		LevelNameNullOrEmpty = 1,
		PreviewImageNotFound = 2,
		PreviewImageInvalidSize = 7,
		ItemFolderNotFound = 3,
		CreateItemFailed = 4,
		InvalidUpdateHandle = 5,
		UpdateItemFailed = 6,
		DeleteItemFailed = 8,
		DownloadItemFailed = 9,
		GetItemInstallInfoFailed = 10,
		SubscribedItemPathNotFound = 11,
		UnsubscribeFailed = 12,
		GetSubscribedItemsQuery = 13,
		GetSubscribeItemInfo = 14,
		GetItemPreviewFile = 0x10,
		MissingIncludedFile = 0xF
	}

	public struct ResultItem
	{
		public PublishedFileId_t id;

		public string title;

		public string path;

		public string previewImagePath;

		public ResultItem(PublishedFileId_t id, string title, string path, string previewImagePath)
		{
			this.id = id;
			this.title = title;
			this.path = path;
			this.previewImagePath = previewImagePath;
		}
	}

	public static int totalPublishedItems;

	public static List<ResultItem> resultItems;

	public static List<WorkshopError> errors;

	public static float itemUploadProgress;

	public static bool gettingSubscribedItemsInProgress;

	public static bool mustAcceptWorkshopLegalAgreement;

	private static bool lastCallResultEnded;

	private static ulong lastBytesProcessed;

	private static UGCUpdateHandle_t updateHandle;

	public static PublishedFileId_t lastPublishedFileId;

	public static PublishedFileId_t lastDownloadFileId;

	private static CallResult<CreateItemResult_t> CreateItemResult;

	private static CallResult<DeleteItemResult_t> DeleteItemResult;

	private static CallResult<SubmitItemUpdateResult_t> SubmitItemUpdateResult;

	private static CallResult<SteamUGCQueryCompleted_t> SteamUGCQueryCompleted;

	private static CallResult<SteamUGCRequestUGCDetailsResult_t> SteamUGCRequestUGCDetailsResult;

	private static CallResult<RemoteStorageDownloadUGCResult_t> RemoteStorageDownloadUGCResult;

	private static Callback<DownloadItemResult_t> DownloadItemResult;

	private static Callback<RemoteStoragePublishedFileUnsubscribed_t> RSPublishedFileUnsubscribed;

	private static Callback<GameOverlayActivated_t> GameOverlayActivated;

	private static Callback<FloatingGamepadTextInputDismissed_t> FloatingGamepadTextInputDismissed;

	public static bool OperationSuccess => errors.Count == 0;

	public static string PreviewImageFolder => Application.persistentDataPath;

	public static void Setup()
	{
		lastBytesProcessed = 0uL;
		DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
		RSPublishedFileUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnPublishedFileUnsubscribed);
		GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnToggleGameOverlay);
		FloatingGamepadTextInputDismissed = Callback<FloatingGamepadTextInputDismissed_t>.Create(OnFloatingGamepadTextInputDismissed);
		RDC.runningOnSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
		if (RDC.runningOnSteamDeck)
		{
			UnityEngine.Debug.Log("runningOnSteamDeck = true");
		}
	}

	public static void OpenWorkshop()
	{
		string text = "https://steamcommunity.com//workshop/browse?appid=977950";
		string str = "&excludedtags[]=";
		if (!ADOBase.ownsTaroDLC)
		{
			text += str;
		}
		text = text.Replace(' ', '+');
		SteamFriends.ActivateGameOverlayToWebPage(text, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Modal);
	}

	public static void OnToggleGameOverlay(GameOverlayActivated_t param)
	{
		Convert.ToBoolean(param.m_bActive);
	}

	public static void ShowItemOnWorkshop(PublishedFileId_t publishedFileId)
	{
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + publishedFileId.m_PublishedFileId.ToString());
	}

	public static bool OverlayEnabled()
	{
		return SteamUtils.IsOverlayEnabled();
	}

	public static bool ShowTextInput()
	{
		return SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 50, 10);
	}

	public static void OnFloatingGamepadTextInputDismissed(FloatingGamepadTextInputDismissed_t pCallback)
	{
		scnCLS instance = scnCLS.instance;
		if (instance != null)
		{
			instance.StartCoroutine(instance.optionsPanels.ToggleSearchMode(search: false));
		}
	}

	public static void Subscribe(PublishedFileId_t publishedFileId)
	{
		SteamUGC.SubscribeItem(publishedFileId);
	}

	public static void Unsubscribe(PublishedFileId_t publishedFileId)
	{
		SteamUGC.UnsubscribeItem(publishedFileId);
	}

	public static void OnPublishedFileUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t param)
	{
		bool flag = param.m_nAppID != SteamIntegration.Instance.gameID.AppID();
	}

	public static IEnumerator GetPublishedItems(int pageNumber = 1)
	{
		RDBaseDll.printem($"pageNumber: {pageNumber}");
		resultItems = new List<ResultItem>();
		errors = new List<WorkshopError>();
		UGCQueryHandle_t queryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_LastUpdatedDesc, SteamUtils.GetAppID(), SteamUtils.GetAppID(), (uint)pageNumber);
		RDBaseDll.printem("pageNumber 2");
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(queryHandle);
		SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create();
		int itemsNumber = 0;
		lastCallResultEnded = false;
		RDBaseDll.printem("pageNumber 2.5");
		SteamUGCQueryCompleted.Set(hAPICall, delegate(SteamUGCQueryCompleted_t param, bool bIOFailure)
		{
			if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
			{
				UnityEngine.Debug.Log($"GetPublishedItems query failed: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
				errors.Add(WorkshopError.GetSubscribedItemsQuery);
			}
			else
			{
				RDBaseDll.printem($"itemsNumber = {param.m_unNumResultsReturned}, itemsTotal = {param.m_unTotalMatchingResults}, cachedData = {param.m_bCachedData}, nextPageCursor = {param.m_rgchNextCursor}");
				itemsNumber = (int)param.m_unNumResultsReturned;
				totalPublishedItems = (int)param.m_unTotalMatchingResults;
			}
			lastCallResultEnded = true;
			RDBaseDll.printem("pageNumber 3");
		});
		RDBaseDll.printem("pageNumber 2.8");
		yield return new WaitUntil(() => lastCallResultEnded);
		RDBaseDll.printem("pageNumber 4");
		if (!OperationSuccess)
		{
			yield break;
		}
		RDBaseDll.printem("pageNumber 5");
		RDBaseDll.printem("here");
		uint i;
		for (i = 0u; i < itemsNumber; i++)
		{
			RDBaseDll.printem("pageNumber 6");
			if (SteamUGC.GetQueryUGCResult(queryHandle, i, out SteamUGCDetails_t itemDetails))
			{
				lastCallResultEnded = false;
				string previewImagePath = Path.Combine(PreviewImageFolder, $"{itemDetails.m_nPublishedFileId}.png");
				SteamAPICall_t hAPICall2 = SteamRemoteStorage.UGCDownloadToLocation(itemDetails.m_hPreviewFile, previewImagePath, 0u);
				RemoteStorageDownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create();
				RemoteStorageDownloadUGCResult.Set(hAPICall2, delegate(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
				{
					if (param.m_nAppID == SteamUtils.GetAppID() && param.m_ulSteamIDOwner == SteamUser.GetSteamID().m_SteamID)
					{
						if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
						{
							UnityEngine.Debug.Log($"UGCDownloadToLocation failed for item {i}: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
							errors.Add(WorkshopError.GetItemPreviewFile);
						}
						else
						{
							RDBaseDll.printem($"item {i}: id = {itemDetails.m_nPublishedFileId}, title = {itemDetails.m_rgchTitle}, previewImagePath = {previewImagePath}");
							resultItems.Add(new ResultItem(itemDetails.m_nPublishedFileId, itemDetails.m_rgchTitle, string.Empty, previewImagePath));
						}
					}
					lastCallResultEnded = true;
				});
				yield return new WaitUntil(() => lastCallResultEnded);
			}
			else
			{
				RDBaseDll.printem($"GetQueryUGCResult failed for index {i}: query handle is invalid or the index is out of bounds");
				errors.Add(WorkshopError.GetSubscribeItemInfo);
			}
		}
		errors = errors.Distinct().ToList();
		RDBaseDll.printem($"completed: {resultItems.Count} items, {errors.Count} errors");
	}

	public static IEnumerator GetSubscribedItems()
	{
		gettingSubscribedItemsInProgress = true;
		try
		{
			errors = new List<WorkshopError>();
			resultItems = new List<ResultItem>();
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
			PublishedFileId_t[] array2 = array;
			foreach (PublishedFileId_t subscribedItemId in array2)
			{
				if (!ItemIsUsable(subscribedItemId) && SteamUGC.DownloadItem(subscribedItemId, bHighPriority: true))
				{
					lastCallResultEnded = false;
					yield return new WaitUntil(() => lastCallResultEnded);
					if (!OperationSuccess)
					{
						continue;
					}
				}
				if (SteamUGC.GetItemInstallInfo(subscribedItemId, out ulong _, out string pchFolder, 1000u, out uint _))
				{
					if (!string.IsNullOrEmpty(pchFolder) && Directory.Exists(pchFolder))
					{
						resultItems.Add(new ResultItem(subscribedItemId, string.Empty, pchFolder, string.Empty));
					}
					else
					{
						errors.Add(WorkshopError.SubscribedItemPathNotFound);
						RDBaseDll.printem($"ERROR Subscribed item {pchFolder} path not found: {!string.IsNullOrEmpty(pchFolder)} && {Directory.Exists(pchFolder)}");
					}
				}
				else
				{
					errors.Add(WorkshopError.GetItemInstallInfoFailed);
					RDBaseDll.printem($"ERROR GetItemInstallInfo failed for {subscribedItemId}");
				}
			}
			errors = errors.Distinct().ToList();
		}
		finally
		{
			gettingSubscribedItemsInProgress = false;
		}
	}

	public static bool ItemIsUsable(PublishedFileId_t publishedFileId)
	{
		EItemState itemState = (EItemState)SteamUGC.GetItemState(publishedFileId);
		if (!itemState.HasFlag(EItemState.k_EItemStateDownloading) && !itemState.HasFlag(EItemState.k_EItemStateDownloadPending))
		{
			return !itemState.HasFlag(EItemState.k_EItemStateNeedsUpdate);
		}
		return false;
	}

	private static void OnDownloadItemResult(DownloadItemResult_t param)
	{
		if (!(param.m_unAppID != SteamIntegration.Instance.gameID.AppID()))
		{
			if (param.m_eResult != EResult.k_EResultOK)
			{
				errors.Add(WorkshopError.DownloadItemFailed);
				RDBaseDll.printem($"OnDownloadItemResult for {param.m_nPublishedFileId} was {param.m_eResult}");
			}
			lastCallResultEnded = true;
		}
	}

	public static void CheckDownloadInfo()
	{
		ulong punBytesDownloaded;
		ulong punBytesTotal;
		bool itemDownloadInfo = SteamUGC.GetItemDownloadInfo(lastDownloadFileId, out punBytesDownloaded, out punBytesTotal);
		if ((float)(double)punBytesDownloaded > 0f && (float)(double)punBytesTotal > 0f && punBytesTotal > punBytesDownloaded)
		{
			RDBaseDll.printem($"Download info: {lastDownloadFileId}, {punBytesDownloaded}, {punBytesTotal}, {itemDownloadInfo}");
		}
	}

	public static IEnumerator UploadToWorkshop(string title, string description, string previewImagePath, string contentPath, string[] tags, PublishedFileId_t updateId = default(PublishedFileId_t), uint[] requiredDLC = null)
	{
		errors = new List<WorkshopError>();
		if (string.IsNullOrEmpty(title))
		{
			errors.Add(WorkshopError.LevelNameNullOrEmpty);
		}
		else
		{
			int num = 127;
			if (Encoding.UTF8.GetByteCount(title) > num)
			{
				byte[] bytes = Encoding.Default.GetBytes(title);
				title = Encoding.UTF8.GetString(bytes, 0, num);
				RDBaseDll.printem($"Trimmed title from {bytes.Length} to {num}: {title}");
			}
		}
		if (string.IsNullOrEmpty(previewImagePath) || !RDFile.Exists(previewImagePath))
		{
			errors.Add(WorkshopError.PreviewImageNotFound);
		}
		else
		{
			long length = new FileInfo(previewImagePath).Length;
			if (length <= 16 || length >= 1000000)
			{
				errors.Add(WorkshopError.PreviewImageInvalidSize);
			}
		}
		if (string.IsNullOrEmpty(contentPath) || !RDDirectory.Exists(contentPath))
		{
			errors.Add(WorkshopError.ItemFolderNotFound);
		}
		if (!OperationSuccess)
		{
			yield break;
		}
		for (int i = 0; i < tags.Length; i++)
		{
			tags[i] = tags[i].Truncate(255);
		}
		if (!string.IsNullOrEmpty(description) && description.Length > 1000)
		{
			description = description.Truncate(1000);
		}
		SteamAPICall_t hAPICall;
		if (updateId == default(PublishedFileId_t))
		{
			hAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
			CreateItemResult = CallResult<CreateItemResult_t>.Create();
			lastCallResultEnded = false;
			CreateItemResult.Set(hAPICall, delegate(CreateItemResult_t param, bool bIOFailure)
			{
				if (!bIOFailure && param.m_eResult == EResult.k_EResultOK)
				{
					lastPublishedFileId = param.m_nPublishedFileId;
				}
				else
				{
					errors.Add(WorkshopError.CreateItemFailed);
					RDBaseDll.printem($"Couldn't create the item in the workshop: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
				}
				lastCallResultEnded = true;
			});
			yield return new WaitUntil(() => lastCallResultEnded);
			if (!OperationSuccess)
			{
				yield break;
			}
		}
		else
		{
			lastPublishedFileId = updateId;
		}
		updateHandle = SteamUGC.StartItemUpdate(SteamIntegration.Instance.gameID.AppID(), lastPublishedFileId);
		ERemoteStoragePublishedFileVisibility eVisibility = ((UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)) && UnityEngine.Input.GetKey(KeyCode.U)) ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted : ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
		if (!SteamUGC.SetItemTitle(updateHandle, title) || (!string.IsNullOrEmpty(description) && !SteamUGC.SetItemDescription(updateHandle, description)) || !SteamUGC.SetItemTags(updateHandle, tags) || !SteamUGC.SetItemPreview(updateHandle, previewImagePath) || !SteamUGC.SetItemContent(updateHandle, contentPath) || !SteamUGC.SetItemVisibility(updateHandle, eVisibility))
		{
			errors.Add(WorkshopError.InvalidUpdateHandle);
			yield return DeleteItem(lastPublishedFileId);
			yield break;
		}
		foreach (uint value in requiredDLC)
		{
			SteamUGC.AddAppDependency(lastPublishedFileId, new AppId_t(value));
		}
		RDBaseDll.printem($"Uploading to workshop with id {lastPublishedFileId}");
		hAPICall = SteamUGC.SubmitItemUpdate(updateHandle, "Item Created");
		SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create();
		lastCallResultEnded = false;
		SubmitItemUpdateResult.Set(hAPICall, delegate(SubmitItemUpdateResult_t param, bool bIOFailure)
		{
			mustAcceptWorkshopLegalAgreement = param.m_bUserNeedsToAcceptWorkshopLegalAgreement;
			if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
			{
				errors.Add(WorkshopError.UpdateItemFailed);
				RDBaseDll.printem($"SubmitItemUpdateResult - any failure: {bIOFailure}, result: {param.m_eResult}," + $" needs to accept legal agreement: {param.m_bUserNeedsToAcceptWorkshopLegalAgreement}");
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
		if (!OperationSuccess)
		{
			yield return DeleteItem(lastPublishedFileId);
		}
		else
		{
			RDBaseDll.printem($"UploadToWorkshop success: {OperationSuccess}, errors: {errors.Count}");
		}
	}

	private static IEnumerator DeleteItem(PublishedFileId_t publishedFileId)
	{
		SteamAPICall_t hAPICall = SteamUGC.DeleteItem(publishedFileId);
		DeleteItemResult = CallResult<DeleteItemResult_t>.Create();
		lastCallResultEnded = false;
		DeleteItemResult.Set(hAPICall, delegate(DeleteItemResult_t param, bool bIOFailure)
		{
			if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
			{
				errors.Add(WorkshopError.DeleteItemFailed);
				RDBaseDll.printem($"Couldn't delete the item in the workshop: bIOFailure = {bIOFailure}, result = {param.m_eResult}, for item = {param.m_nPublishedFileId}");
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
	}

	public static void CheckUploadInfo()
	{
		ulong punBytesProcessed;
		ulong punBytesTotal;
		EItemUpdateStatus itemUpdateProgress = SteamUGC.GetItemUpdateProgress(updateHandle, out punBytesProcessed, out punBytesTotal);
		if ((float)(double)punBytesProcessed > 0f && (float)(double)punBytesTotal > 0f && punBytesTotal > punBytesProcessed && lastBytesProcessed != punBytesProcessed)
		{
			lastBytesProcessed = punBytesProcessed;
			itemUploadProgress = (float)(double)punBytesProcessed / ((float)(double)punBytesTotal * 1f);
			RDBaseDll.printem(string.Format("{0} upload info: status = {1}, progress = {2}%", updateHandle, itemUpdateProgress, (itemUploadProgress * 100f).ToString("00.00")));
		}
	}
}
