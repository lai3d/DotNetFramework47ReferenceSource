#pragma once

#ifndef __XPSPRINTHELPER_H__
#define __XPSPRINTHELPER_H__

typedef HRESULT (WINAPI *STARTXPSPRINTJOBFN)(
	/* [string][in] */ __RPC__in_string LPCWSTR printerName,
	/* [string][in] */ __RPC__in_string LPCWSTR jobName,
	/* [string][in] */ __RPC__in_string LPCWSTR outputFileName,
	/* [in] */ __RPC__in HANDLE progressEvent,
	/* [in] */ __RPC__in HANDLE completionEvent,
	/* [size_is][in] */ __RPC__in_ecount_full(printablePagesOnCount) UINT8 *printablePagesOn,
	/* [in] */ UINT32 printablePagesOnCount,
	/* [out] */ __RPC__deref_out_opt IXpsPrintJob **xpsPrintJob,
	/* [out] */ __RPC__deref_out_opt IXpsPrintJobStream **documentStream,
	/* [out] */ __RPC__deref_out_opt IXpsPrintJobStream **printTicketStream
	);

EXTERN_C
BOOL WINAPI IsStartXpsPrintJobSupported(VOID);

// Helper function to call xpsprint.dll!StartXpsPrintJob if it exists
// otherwise it returns E_NOIMPL
EXTERN_C
HRESULT WINAPI LateBoundStartXpsPrintJob(
/* [string][in] */ __RPC__in_string LPCWSTR printerName,
/* [string][in] */ __RPC__in_string LPCWSTR jobName,
/* [string][in] */ __RPC__in_string LPCWSTR outputFileName,
/* [in] */ __RPC__in HANDLE progressEvent,
/* [in] */ __RPC__in HANDLE completionEvent,
/* [size_is][in] */ __RPC__in_ecount_full(printablePagesOnCount) UINT8 *printablePagesOn,
/* [in] */ UINT32 printablePagesOnCount,
/* [out] */ __RPC__deref_out_opt IXpsPrintJob **xpsPrintJob,
/* [out] */ __RPC__deref_out_opt IXpsPrintJobStream **documentStream,
/* [out] */ __RPC__deref_out_opt IXpsPrintJobStream **printTicketStream
);

EXTERN_C
BOOL WINAPI IsPrintPackageTargetSupported(VOID);

// Helper function to call Print Document Package API Interfaces if it exists
// otherwise it returns E_NOIMPL
EXTERN_C
HRESULT WINAPI PrintToPackageTarget(
/* [string][in] */ __RPC__in_string LPCWSTR printerName,
/* [string][in] */ __RPC__in_string LPCWSTR jobName,
/* [in] */ __RPC__in_opt IStream *jobPrintTicketStream,
/* [out] */ __RPC__deref_out_opt IPrintDocumentPackageTarget  **printDocPackageTarget,
/* [out] */ __RPC__deref_out_opt IXpsDocumentPackageTarget  **xpsPackageTarget);

#endif // __XPSPRINTHELPER_H__
