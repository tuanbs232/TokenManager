#ifndef _WIN32_WINNT         
#define _WIN32_WINNT 0x0501
#endif

#ifndef NTDDI_VERSION
#define NTDDI_VERSION 0x05010300 /* XP SP3 minimum */
#endif

#ifndef UNICODE
#define UNICODE
#endif

#ifndef _UNICODE
#define _UNICODE
#endif

#include <windows.h>
#include <wincrypt.h>
#include <stdio.h>
#include <tchar.h>
#include <string>
#include <list>
#include <strsafe.h>

#pragma comment (lib, "crypt32.lib")

extern "C"
{

	bool AddCertToStore(HCERTSTORE hCertStore,
		LPBYTE pbCertificate,
		DWORD cbCertificate,
		LPWSTR szCspName,
		LPWSTR szContainer,
		DWORD dwProvType,
		DWORD dwKeySpec)
	{
		try {
			bool bRet = false;
			PCCERT_CONTEXT pCertContext = NULL;
			if (!CertAddEncodedCertificateToStore(hCertStore, X509_ASN_ENCODING,
				pbCertificate, cbCertificate, CERT_STORE_ADD_REPLACE_EXISTING, &pCertContext))
			{
				return false;
			}

			CRYPT_KEY_PROV_INFO CryptKeyProvInfo;
			CryptKeyProvInfo.pwszContainerName = szContainer;
			CryptKeyProvInfo.pwszProvName = szCspName;
			CryptKeyProvInfo.dwProvType = dwProvType;
			CryptKeyProvInfo.dwFlags = 0;
			CryptKeyProvInfo.cProvParam = 0;
			CryptKeyProvInfo.rgProvParam = 0;
			CryptKeyProvInfo.dwKeySpec = dwKeySpec;

			if (CertSetCertificateContextProperty(pCertContext, CERT_KEY_PROV_INFO_PROP_ID, 0, &CryptKeyProvInfo))
				bRet = true;

			CertFreeCertificateContext(pCertContext);
			return bRet;
		}
		catch (int code) {
			return false;
		}
	}

	bool LoadCertificate(HCERTSTORE hCertStore,
		HCRYPTPROV hProv,
		LPWSTR szCspName,
		LPWSTR szContainer,
		DWORD dwProvType,
		DWORD dwKeySpec)
	{
		try {
			bool bRet = false;
			HCRYPTKEY hKey;
			LPBYTE pbCertificate = NULL;
			DWORD cbCertificate = 0;
			BOOL getUserKey = CryptGetUserKey(hProv, dwKeySpec, &hKey);
			if (getUserKey)
			{
				if (CryptGetKeyParam(hKey, KP_CERTIFICATE, NULL, &cbCertificate, 0))
				{
					pbCertificate = new BYTE[cbCertificate];
					if (CryptGetKeyParam(hKey, KP_CERTIFICATE, pbCertificate, &cbCertificate, 0))
					{
						bRet = AddCertToStore(hCertStore, pbCertificate, cbCertificate, szCspName, szContainer, dwProvType, dwKeySpec);
					}
					delete[] pbCertificate;
				}
				CryptDestroyKey(hKey);
			}
			else {
				DWORD error = GetLastError();
			}

			return bRet;
		}
		catch (int code) {
			return false;
		}
	}

	void UnloadCertificates(HCERTSTORE hCertStore, LPCWSTR szCspName, LPCWSTR szContainerName, int& count)
	{
		// enumerate all certificates in store in order to find the matching one
		try {
			PCCERT_CONTEXT pCertContext = NULL;
			LPBYTE pbData = NULL;
			DWORD cbData = 0;
			while ((pCertContext = CertEnumCertificatesInStore(hCertStore, pCertContext)))
			{
				bool bMatching = false;
				cbData = 0;
				if (CertGetCertificateContextProperty(pCertContext, CERT_KEY_PROV_INFO_PROP_ID, NULL, &cbData))
				{
					pbData = new BYTE[cbData];
					if (CertGetCertificateContextProperty(pCertContext, CERT_KEY_PROV_INFO_PROP_ID, pbData, &cbData))
					{
						CRYPT_KEY_PROV_INFO* pInfo = (CRYPT_KEY_PROV_INFO*)pbData;
						if ((pInfo->pwszProvName && (0 == wcscmp(szCspName, pInfo->pwszProvName)))
							&& (!szContainerName || (pInfo->pwszContainerName && (0 == wcscmp(szContainerName, pInfo->pwszContainerName))))
							)
						{
							bMatching = true;
						}

					}

					delete[] pbData;
				}

				if (bMatching)
				{
					PCCERT_CONTEXT pCopyCtx = CertDuplicateCertificateContext(pCertContext);
					if (CertDeleteCertificateFromStore(pCopyCtx))
						count++;
				}
			}
		}
		catch (int code) {
			return;
		}
	}
	//End of UnloadCertificate()


	//-------------------------------------------------------------------------
	//Functions for P/invoke
	__declspec(dllexport) int C_UnloadAllCertificate(TCHAR* cspProv)
	{
		bool bUseDefaultContainer = true;
		TCHAR szCspName[MAX_PATH] = { 0 };
		TCHAR szContainerName[MAX_PATH] = { 0 };
		TCHAR szProvName[MAX_PATH];
		HCRYPTPROV hProv;
		BOOL bRet;
		DWORD dwProvType = 0;
		DWORD cbProvName = sizeof(szProvName);
		DWORD dwIndex = 0, dwFlags = CRYPT_SILENT;
		HCERTSTORE hCertStore = NULL;
		int count = 0;

		dwFlags |= CRYPT_VERIFYCONTEXT;

		//Check provider name parameter
		StringCbCopy(szCspName, sizeof(szCspName), cspProv);
		if (!_tcslen(szCspName))
		{
			return 2;
		}

		//Find registered provider match name parameter
		do
		{
			DWORD dwValue = 0;
			cbProvName = sizeof(szProvName);
			bRet = CryptEnumProviders(dwIndex++, NULL, 0, &dwValue, szProvName, &cbProvName);
			if (bRet && (0 == _tcscmp(szProvName, szCspName)))
			{
				dwProvType = dwValue;
				break;
			}
		} while (bRet);

		if (dwProvType == 0)
		{
			return 3;
		}

		//Open Window-MY store
		hCertStore = CertOpenSystemStore(NULL, _T("MY"));
		if (!hCertStore)
		{
			return 4;
		}

		UnloadCertificates(hCertStore, szCspName, bUseDefaultContainer ? NULL : szContainerName, count);

		if (hCertStore)
			CertCloseStore(hCertStore, 0);

		return 0;
	}
	__declspec(dllexport) int C_LoadAllCertToStore(TCHAR* cspProv)
	{
		bool bUseDefaultContainer = true;
		TCHAR szCspName[MAX_PATH] = { 0 };
		TCHAR szContainerName[MAX_PATH] = { 0 };
		TCHAR szProvName[MAX_PATH];
		HCRYPTPROV hProv;
		BOOL bRet;
		DWORD dwProvType = 0;
		DWORD cbProvName = sizeof(szProvName);
		DWORD dwIndex = 0, dwFlags = CRYPT_SILENT;
		HCERTSTORE hCertStore = NULL;
		int count = 0;

		dwFlags &= CRYPT_MACHINE_KEYSET;

		//Check provider name parameter
		StringCbCopy(szCspName, sizeof(szCspName), cspProv);
		if (!_tcslen(szCspName))
		{
			return 2;
		}

		//Find registered provider match name parameter
		do
		{
			DWORD dwValue = 0;
			cbProvName = sizeof(szProvName);
			bRet = CryptEnumProviders(dwIndex++, NULL, 0, &dwValue, szProvName, &cbProvName);
			if (bRet && (0 == _tcscmp(szProvName, szCspName)))
			{
				dwProvType = dwValue;
				break;
			}
		} while (bRet);

		if (dwProvType == 0)
		{
			return 3;
		}

		//Open Window-MY store
		hCertStore = CertOpenSystemStore(NULL, _T("MY"));
		if (!hCertStore)
		{
			//Cannot open Window-MY store
			return 4;
		}

		
		bRet = CryptAcquireContext(&hProv, NULL, szCspName, dwProvType, dwFlags);
		if (!bRet) {
			//Cannot acquire cryptography context
			return 5;
		}

		// enumerate all containers
		DWORD dwEnumFlags = CRYPT_FIRST;
		std::list<std::wstring> listContainers;
		CHAR szValue[1024];
		WCHAR wszValue[1024];
		DWORD cbValue;

		while (true)
		{
			cbValue = sizeof(szValue);
			if (CryptGetProvParam(hProv, PP_ENUMCONTAINERS, (BYTE*)szValue, &cbValue, dwEnumFlags))
			{
				if (MultiByteToWideChar(CP_ACP, 0, szValue, -1, wszValue, ARRAYSIZE(wszValue)) > 1)
					listContainers.push_back(wszValue);
			}
			else
				break;

			dwEnumFlags = CRYPT_NEXT;
		}

		if (listContainers.empty())
		{
			//No key container name was found
			return 6;
		}
		else
		{
			// iterate over all containers
			CryptReleaseContext(hProv, 0);
			hProv = NULL;

			for (std::list<std::wstring>::iterator It = listContainers.begin(); It != listContainers.end(); It++)
			{
				StringCbCopy(wszValue, sizeof(wszValue), It->c_str());
				bRet = CryptAcquireContext(&hProv, wszValue, szCspName, dwProvType, CRYPT_SILENT);
				if (bRet)
				{
					if (LoadCertificate(hCertStore, hProv, szCspName, wszValue, dwProvType, AT_KEYEXCHANGE))
						count++;
					if (LoadCertificate(hCertStore, hProv, szCspName, wszValue, dwProvType, AT_SIGNATURE))
						count++;

					CryptReleaseContext(hProv, 0);
					hProv = NULL;
				}
			}
		}

		if (hCertStore)
			CertCloseStore(hCertStore, 0);
		if (hProv)
			CryptReleaseContext(hProv, 0);

		return 0;
	}

	//-------------------------------------------------------------------------
}
