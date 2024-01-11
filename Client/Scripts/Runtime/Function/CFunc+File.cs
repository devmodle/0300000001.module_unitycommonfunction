using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.IO;

/** 파일 함수 */
public static partial class CFunc {
	#region 클래스 함수
	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, 
		string a_oDestPath, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oBytes = CFunc.ReadBytes(a_oSrcPath, false);
			CFunc.WriteBytes(a_oDestPath, oBytes, false, null, a_bIsEnableAssert);
		}
	}

	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, 
		string a_oDestPath, string a_oIgnoreToken, System.Text.Encoding a_oEncoding = null, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrLines = CFunc.ReadStrLines(a_oSrcPath, a_oEncoding ?? System.Text.Encoding.Default);
			var oStrBuilder = new System.Text.StringBuilder();

			for(int i = 0; i < oStrLines.Length; ++i) {
				// 문자열이 유효 할 경우
				if(oStrLines[i] != null && !oStrLines[i].Contains(a_oIgnoreToken)) {
					oStrBuilder.AppendLine(oStrLines[i]);
				}
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString(), false, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
		}
	}

	/** 파일을 복사한다 */
	public static void CopyFile(string a_oSrcPath, 
		string a_oDestPath, string a_oTarget, string a_oReplace, System.Text.Encoding a_oEncoding = null, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 파일 복사가 가능 할 경우
		if((bIsValid && File.Exists(a_oSrcPath)) && (a_bIsOverwrite || !File.Exists(a_oDestPath))) {
			var oStrLines = CFunc.ReadStrLines(a_oSrcPath, a_oEncoding ?? System.Text.Encoding.Default);
			var oStrBuilder = new System.Text.StringBuilder();

			for(int i = 0; i < oStrLines.Length; ++i) {
				// 문자열이 유효 할 경우
				if(oStrLines[i] != null) {
					oStrBuilder.AppendLine(oStrLines[i].Replace(a_oTarget, a_oReplace));
				}
			}

			CFunc.WriteStr(a_oDestPath, oStrBuilder.ToString(), false, a_oEncoding ?? System.Text.Encoding.Default, a_bIsEnableAssert);
		}
	}

	/** 디렉토리를 복사한다 */
	public static void CopyDir(string a_oSrcPath, 
		string a_oDestPath, bool a_bIsOverwrite = true, bool a_bIsEnableAssert = true) {

		CAccess.Assert(!a_bIsEnableAssert || (a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid()));
		bool bIsValid = a_oSrcPath.ExIsValid() && a_oDestPath.ExIsValid();

		// 디렉토리 복사가 가능 할 경우
		if((bIsValid && Directory.Exists(a_oSrcPath)) && (a_bIsOverwrite || !Directory.Exists(a_oDestPath))) {
			CFactory.RemoveDir(a_oDestPath);

			CAccess.EnumerateDirectories(a_oSrcPath, (a_oDirPathList, a_oFilePathList) => {
				for(int i = 0; i < a_oFilePathList.Count; ++i) {
					string oDestFilePath = a_oFilePathList[i].Replace(a_oSrcPath, a_oDestPath);
					CFunc.CopyFile(a_oFilePathList[i], oDestFilePath, a_bIsOverwrite);
				}

				return true;
			});
		}
	}
	#endregion // 클래스 함수
}
