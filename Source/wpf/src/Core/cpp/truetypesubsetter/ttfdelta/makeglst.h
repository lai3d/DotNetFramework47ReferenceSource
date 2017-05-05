/*
  * MakeGLst.h: Interface file for MakeGLst.c - Written by Louise Pathe
  *
  * Copyright 1990-1997. Microsoft Corporation.
  * 
  */
  
#ifndef MAKEGLIST_DOT_H_DEFINED
#define MAKEGLIST_DOT_H_DEFINED        

[System::Security::SecurityCritical]
int16 MakeKeepGlyphList(
TTFACC_FILEBUFFERINFO * pInputBufferInfo,
CONST uint16 usListType, /* 0 = character list, 1 = glyph list */
CONST uint16 usPlatform,
CONST uint16 usEncoding,
CONST CHAR_ID *pulKeepCharCodeList,
CONST uint16 usCharListCount,
uint8 *puchKeepGlyphList, /* pointer to an array of characters representing glyphs 0-usGlyphListCount. */
CONST uint16 usGlyphListCount,
uint16 *pusMaxGlyphIndexUsed,
uint16 *pusGlyphKeepCount,
BOOL bAddRelatedGlyphs 
);
#endif /* MAKEGLIST_DOT_H_DEFINED */
