using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//! 기본 함수
public static partial class CFunc {
	#region 클래스 함수
	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, string a_oDestFilepath, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcFilepath);
			CFunc.WriteBytes(a_oDestFilepath, oBytes);
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, 
		string a_oDestFilepath, string a_oIgnore, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid() && a_oIgnore.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcFilepath, a_oEncoding);

			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = 0; i < oStringLines.Length; ++i) {
					if(!oStringLines[i].Contains(a_oIgnore)) {
						oStringBuilder.AppendLine(oStringLines[i]);
					}
				}

				CFunc.WriteString(a_oDestFilepath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 파일을 복사한다
	public static void CopyFile(string a_oSrcFilepath, 
		string a_oDestFilepath, string a_oSearch, string a_oReplace, System.Text.Encoding a_oEncoding, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcFilepath.ExIsValid() && a_oDestFilepath.ExIsValid() && a_oSearch.ExIsValid() && a_oReplace.ExIsValid());

		// 파일 복사가 가능 할 경우
		if(File.Exists(a_oSrcFilepath) && (a_bIsOverwrite || !File.Exists(a_oDestFilepath))) {
			var oStringLines = CFunc.ReadStringLines(a_oSrcFilepath, a_oEncoding);

			if(oStringLines.ExIsValid()) {
				var oStringBuilder = new System.Text.StringBuilder();

				for(int i = 0; i < oStringLines.Length; ++i) {
					string oString = !oStringLines[i].ExIsValid() ? string.Empty
						: oStringLines[i].ExGetReplaceString(a_oSearch, a_oReplace, short.MaxValue);
						
					oStringBuilder.AppendLine(oString);
				}

				CFunc.WriteString(a_oDestFilepath, oStringBuilder.ToString(), a_oEncoding);
			}
		}
	}

	//! 디렉토리를 복사한다
	public static void CopyDirectory(string a_oSrcPath, string a_oDestPath, bool a_bIsOverwrite = true) {
		CAccess.Assert(a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid());

		// 디렉토리 복사가 가능 할 경우
		if(Directory.Exists(a_oSrcPath)) {
			// 파일을 복사한다
			if(a_bIsOverwrite || !Directory.Exists(a_oDestPath)) {
				CAccess.RemoveDirectory(a_oDestPath);
				Directory.CreateDirectory(a_oDestPath);

				var oFiles = Directory.GetFiles(a_oSrcPath);

				for(int i = 0; i < oFiles.Length; ++i) {
					string oFilename = Path.GetFileName(oFiles[i]);
					CFunc.CopyFile(oFiles[i], Path.Combine(a_oDestPath, oFilename), a_bIsOverwrite);
				}
			}

			// 하위 디렉토리를 복사한다 {
			var oDirectories = Directory.GetDirectories(a_oSrcPath);

			for(int i = 0; i < oDirectories.Length; ++i) {
				string oDirectoryName = Path.GetFileNameWithoutExtension(oDirectories[i]);
				CFunc.CopyDirectory(oDirectories[i], Path.Combine(a_oDestPath, oDirectoryName), a_bIsOverwrite);
			}
			// 하위 디렉토리를 복사한다 }
		}
	}

	//! 바이트를 읽어들인다
	public static byte[] ReadBytes(string a_oFilepath) {
		CAccess.Assert(a_oFilepath.ExIsValid());
		return File.Exists(a_oFilepath) ? File.ReadAllBytes(a_oFilepath) : null;
	}

	//! 보안 바이트를 읽어들인다
	public static byte[] ReadSecurityBytes(string a_oFilepath) {
		var oBytes = CFunc.ReadBytes(a_oFilepath);
		return (oBytes != null) ? System.Convert.FromBase64String(System.Text.Encoding.Default.GetString(oBytes)) : null;
	}

	//! 문자열을 읽어들인다
	public static string ReadString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		return File.Exists(a_oFilepath) ? File.ReadAllText(a_oFilepath, a_oEncoding) : string.Empty;
	}

	//! 문자열 라인을 읽어들인다
	public static string[] ReadStringLines(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		return File.ReadAllLines(a_oFilepath, a_oEncoding);
	}

	//! 보안 문자열을 읽어들인다
	public static string ReadSecurityString(string a_oFilepath, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oFilepath.ExIsValid());
		var oBytes = CFunc.ReadSecurityBytes(a_oFilepath);

		return (oBytes != null) ? a_oEncoding.GetString(oBytes) : string.Empty;
	}

	//! 바이트를 기록한다
	public static void WriteBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.EMPTY_STRING) {
		using(var oWriteStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteBytes(oWriteStream, a_oBytes);
		}
	}

	//! 바이트를 기록한다
	public static void WriteBytes(FileStream a_oWriteStream, byte[] a_oBytes) {
		CAccess.Assert(a_oBytes.ExIsValid());

		a_oWriteStream?.Write(a_oBytes, 0, a_oBytes.Length);
		a_oWriteStream?.Flush(true);
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(string a_oFilepath,
		byte[] a_oBytes, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.EMPTY_STRING) {
		using(var oWriteStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteSecurityBytes(oWriteStream, a_oBytes);
		}
	}

	//! 보안 바이트를 기록한다
	public static void WriteSecurityBytes(FileStream a_oWriteStream, byte[] a_oBytes) {
		CAccess.Assert(a_oBytes.ExIsValid());
		var oString = System.Convert.ToBase64String(a_oBytes, 0, a_oBytes.Length);

		CFunc.WriteBytes(a_oWriteStream, System.Text.Encoding.Default.GetBytes(oString));
	}

	//! 문자열을 기록한다
	public static void WriteString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.EMPTY_STRING) {
		using(var oWriteStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteString(oWriteStream, a_oString, a_oEncoding);
		}
	}

	//! 문자열을 기록한다
	public static void WriteString(FileStream a_oWriteStream, string a_oString, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oString.ExIsValid());
		CFunc.WriteBytes(a_oWriteStream, a_oEncoding.GetBytes(a_oString));
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(string a_oFilepath,
		string a_oString, System.Text.Encoding a_oEncoding, bool a_bIsAutoCreateDirectory = true, bool a_bIsAutoBackup = false, string a_oBackupDirectoryName = KCDefine.EMPTY_STRING) {
		using(var oWriteStream = CAccess.GetWriteStream(a_oFilepath, a_bIsAutoCreateDirectory, a_bIsAutoBackup, a_oBackupDirectoryName)) {
			CFunc.WriteSecurityString(oWriteStream, a_oString, a_oEncoding);
		}
	}

	//! 보안 문자열을 기록한다
	public static void WriteSecurityString(FileStream a_oWriteStream, string a_oString, System.Text.Encoding a_oEncoding) {
		CAccess.Assert(a_oEncoding != null && a_oString.ExIsValid());
		CFunc.WriteSecurityBytes(a_oWriteStream, a_oEncoding.GetBytes(a_oString));
	}

	//! URL 을 개방한다
	public static void OpenURL(string a_oURL) {
		CAccess.Assert(a_oURL.ExIsValid());
		Application.OpenURL(a_oURL);
	}

	//! 메일을 전송한다
	public static void SendMail(string a_oRecipient, string a_oTitle, string a_oMsg) {
		CAccess.Assert(a_oTitle != null && a_oMsg != null && a_oRecipient.ExIsValid());

		string oURL = string.Format(KCDefine.MAIL_URL_FORMAT,
			a_oRecipient, System.Uri.EscapeUriString(a_oTitle), System.Uri.EscapeUriString(a_oMsg));

		CFunc.OpenURL(oURL);
	}

	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, params object[] a_oParams) {
		CFunc.ShowLog(a_oFormat, KCDefine.LOG_COLOR_INFO, a_oParams);
	}

	//! 로그를 출력한다
	[Conditional("DEBUG"), Conditional("DEVELOPMENT_BUILD")]
	public static void ShowLog(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Log);
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, params object[] a_oParams) {
		CFunc.ShowLogWarning(a_oFormat, KCDefine.LOG_COLOR_WARNING, a_oParams);
	}

	//! 경고 로그를 출력한다
	public static void ShowLogWarning(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Warning);
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, params object[] a_oParams) {
		CFunc.ShowLogError(a_oFormat, KCDefine.LOG_COLOR_ERROR, a_oParams);
	}

	//! 에러 로그를 출력한다
	public static void ShowLogError(string a_oFormat, Color a_stColor, params object[] a_oParams) {
		string oFormat = a_oFormat.ExGetColorFormatString(a_stColor);
		CFunc.DoShowLog(string.Format(oFormat, a_oParams), LogType.Error);
	}
	
	//! 함수를 지연 호출한다
	public static void LateCallFunc(MonoBehaviour a_oComponent,
		System.Action<MonoBehaviour, object[]> a_oCallback, object[] a_oParams = null) {
		a_oComponent?.StartCoroutine(CFunc.DoLateCallFunc(a_oComponent, a_oCallback, a_oParams));
	}

	//! 함수를 지연 호출한다
	public static void LateCallFunc(MonoBehaviour a_oComponent,
		float a_fDelay, System.Action<MonoBehaviour, object[]> a_oCallback, bool a_bIsRealtime = false, object[] a_oParams = null) {
		a_oComponent?.StartCoroutine(CFunc.DoLateCallFunc(a_oComponent, a_fDelay, a_oCallback, a_bIsRealtime, a_oParams));
	}

	//! 함수를 반복 호출한다
	public static void RepeatCallFunc(MonoBehaviour a_oComponent,
		float a_fDeltaTime, float a_fMaxDeltaTime, System.Func<MonoBehaviour, object[], bool, bool> a_oCallback, bool a_bIsRealtime = false, object[] a_oParams = null) {
		CAccess.Assert(a_oCallback != null);
		a_oComponent?.StartCoroutine(CFunc.DoRepeatCallFunc(a_oComponent, a_fDeltaTime, a_fMaxDeltaTime, a_oCallback, a_bIsRealtime, a_oParams));
	}

	//! 비동기 작업을 대기한다
	public static void WaitAsyncTask(Task a_oTask, System.Action<Task> a_oCallback) {
		CAccess.Assert(a_oTask != null);

		a_oTask.ContinueWith((a_oContinueTask) => {
			a_oCallback?.Invoke(a_oContinueTask);
		});
	}

	//! 비동기 작업을 대기한다
	public static IEnumerator WaitAsyncOperation(AsyncOperation a_oAsyncOperation, System.Action<AsyncOperation, bool> a_oCallback, bool a_bIsRealtime = false) {
		CAccess.Assert(a_oAsyncOperation != null);

		do {
			yield return CFactory.CreateWaitForSeconds(KCDefine.DELTA_TIME_ASYNC_OPERATION, a_bIsRealtime);
			a_oCallback?.Invoke(a_oAsyncOperation, false);
		} while(!a_oAsyncOperation.isDone);

		yield return CFactory.CreateWaitForSeconds(KCDefine.DELTA_TIME_ASYNC_OPERATION, a_bIsRealtime);
		a_oCallback?.Invoke(a_oAsyncOperation, true);
	}

	//! 정수 랜덤 값을 생성한다
	public static int[] MakeIntRandomValues(int a_nMin, int a_nMax, int a_nNumValues) {
		CAccess.Assert(a_nMin <= a_nMax);

		return CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => {
			return Random.Range(a_nMin, a_nMax + 1);
		});
	}

	//! 실수 랜덤 값을 생성한다
	public static float[] MakeFloatRandomValues(float a_fMin, float a_fMax, int a_nNumValues) {
		CAccess.Assert(a_fMin <= a_fMax);

		return CFunc.MakeValues<float>(a_nNumValues, (a_nIndex) => {
			return Random.Range(a_fMin, a_fMax);
		});
	}

	//! 정수 랜덤 분할 값을 생성한다
	public static int[] MakeIntRandomSplitValues(int a_nValue, int a_nNumValues) {
		CAccess.Assert(a_nNumValues >= 1);
		int nSumValue = 0;

		var oValues = CFunc.MakeValues<int>(a_nNumValues, (a_nIndex) => {
			int nValue = a_nValue / a_nNumValues;
			nSumValue += nValue;

			return (a_nIndex < a_nNumValues - 1) ? nValue : a_nValue - nSumValue;
		});

		for(int i = 0; i < oValues.Length; ++i) {
			int nIndex = Random.Range(0, oValues.Length);

			if(oValues[i] > 1 && oValues[nIndex] > 1) {
				oValues[i] += 1;
				oValues[nIndex] -= 1;
			}
		}

		return oValues;
	}

	//! 로그를 출력한다
	private static void DoShowLog(string a_oLog, LogType a_eLogType) {
		if(a_eLogType == LogType.Error) {
			UnityEngine.Debug.LogError(a_oLog);
		} else if(a_eLogType == LogType.Warning) {
			UnityEngine.Debug.LogWarning(a_oLog);
		} else {
			UnityEngine.Debug.Log(a_oLog);
		}
	}

	//! 함수를 지연 호출한다
	private static IEnumerator DoLateCallFunc(MonoBehaviour a_oComponent,
		System.Action<MonoBehaviour, object[]> a_oCallback, object[] a_oParams) {
		yield return new WaitForEndOfFrame();
		a_oCallback?.Invoke(a_oComponent, a_oParams);
	}

	//! 함수를 지연 호출한다
	private static IEnumerator DoLateCallFunc(MonoBehaviour a_oComponent,
		float a_fDelay, System.Action<MonoBehaviour, object[]> a_oCallback, bool a_bIsRealtime, object[] a_oParams) {
		yield return CFactory.CreateWaitForSeconds(a_fDelay, a_bIsRealtime);
		a_oCallback?.Invoke(a_oComponent, a_oParams);
	}

	//! 함수를 반복 호출한다
	private static IEnumerator DoRepeatCallFunc(MonoBehaviour a_oComponent,
		float a_fDeltaTime, double a_dblMaxDeltaTime, System.Func<MonoBehaviour, object[], bool, bool> a_oCallback, bool a_bIsRealtime, object[] a_oParams) {
		var stStartTime = System.DateTime.Now;
		System.TimeSpan stDeltaTime;

		do {
			yield return CFactory.CreateWaitForSeconds(a_fDeltaTime, a_bIsRealtime);
			stDeltaTime = System.DateTime.Now - stStartTime;
		} while(a_oCallback(a_oComponent, a_oParams, false) && stDeltaTime.TotalSeconds.ExIsLess(a_dblMaxDeltaTime));

		a_oCallback(a_oComponent, a_oParams, true);
	}
	#endregion			// 클래스 함수

	#region 제네릭 클래스 함수
	//! 값을 교환한다
	public static void Swap<T>(ref T a_tLhs, ref T a_tRhs) {
		T tTemp = a_tLhs;
		a_tLhs = a_tRhs;
		a_tRhs = tTemp;
	}

	//! 비동기 작업을 대기한다
	public static void WaitAsyncTask<T>(Task<T> a_oTask, System.Action<Task<T>> a_oCallback) {
		CAccess.Assert(a_oTask != null);

		a_oTask.ContinueWith((a_oContinueTask) => {
			a_oCallback?.Invoke(a_oContinueTask);
		});
	}

	//! 값을 생성한다
	public static T[] MakeValues<T>(int a_nNumValues, System.Func<int, T> a_oCallback) {
		CAccess.Assert(a_oCallback != null && a_nNumValues >= 1);
		var oValues = new T[a_nNumValues];

		for(int i = 0; i < a_nNumValues; ++i) {
			oValues[i] = a_oCallback.Invoke(i);
		}

		return oValues;
	}

	//! 섞인 값을 생성한다
	public static T[] MakeShuffleValues<T>(int a_nNumValues, System.Func<int, T> a_oCallback) {
		var oValues = CFunc.MakeValues<T>(a_nNumValues, a_oCallback);
		oValues.ExShuffle();

		return oValues;
	}	
	#endregion			// 제네릭 클래스 함수
}
