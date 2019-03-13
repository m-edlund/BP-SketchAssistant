/*
 *  Serial.h
 *
 *  Copyright (C)
 *  Honda Research Institute Europe GmbH
 *  Carl-Legien-Str. 30
 *  63073 Offenbach/Main
 *  Germany
 *
 *  UNPUBLISHED PROPRIETARY MATERIAL.
 *  ALL RIGHTS RESERVED.
 *
 */

#pragma once
#include <stdbool.h>
#include <stdint.h>
//#include <windows.h>

typedef struct Serial
{
    bool valid;
    char* port;
    HANDLE hComm;
    bool serialStatus;
} Serial;

Serial* Serial_new();
void Serial_init(Serial* self, char* port);
void Serial_clear(Serial* self);
void Serial_delete(Serial* self);

void Serial_sendMessage(Serial* self, unsigned char* message, int size);