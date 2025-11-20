// SPDX-FileCopyrightText: 2025 Carl Zeiss Microscopy GmbH
//
// SPDX-License-Identifier: MIT

namespace CziCheckSharp;

public record class Finding(
    FindingSeverity Severity,
    string Description,
    string Details)
{
}