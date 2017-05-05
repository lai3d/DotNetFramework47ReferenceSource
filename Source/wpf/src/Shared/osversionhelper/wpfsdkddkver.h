#pragma once 
#if !defined(_WPF_SDKDDKVER_H_)
#define _WPF_SDKDDKVER_H_

// NOTE: 
// 
// We define some constants not available in our headers
// These are from Win10's \public\shared\inc\sdkddkver.h
//
// If any of these constants are already defined, then 
// it suggests that a newer sdkddkver.h has been introduced
// into the build environment, and we should be adjusting 
// this file (or removing it entirely)

#if defined(_WIN32_WINNT_WIN10)


#error _WIN32_WINNT_* macros already present in standard SDK headers are being redefined. See notes in this header.

#endif 

//
// _WIN32_WINNT version constants
//
#define _WIN32_WINNT_WIN10                  0x0A00 /* ABRACADABRA_THRESHOLD*/

#define _TH1_BUILD_NUMBER                   10240
#define _TH2_BUILD_NUMBER                   10586
#define _RS1_BUILD_NUMBER                   14393

#endif // _WPF_SDKDDKVER_H_
