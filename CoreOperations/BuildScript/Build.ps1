###################################################################################################
###
###  build.ps1 -- PowerShell script called in Visual Studio post-build step.
###
###  The post-build step is very simple and looks like this:
###    powershell.exe "$(ProjectDir)\Solution\Build\build.ps1"
param
(
[string]$targetDir,
[string]$targetFile,
[string]$targetName
)
$ErrorActionPreference = "Stop"

try {

$targetDir = $targetDir.Substring(0, $targetDir.length - 3)
$mergeFolderName = "pluginsmerged"
$mergeFolder = "$targetDir\$mergeFolderName"
$libFolder = $mergeFolder + "\pluginslib"
$buildFolder = Split-Path -path $MyInvocation.MyCommand.Definition


# Clean up the temp folder from possible previous builds
if (Test-Path $mergeFolder)
{
	Remove-Item "$mergeFolder" -Recurse -Force -ErrorAction Stop
}
mkdir $mergeFolder


### Copy all of the output files to the merged folder for ILMerging
Get-ChildItem $targetDir -Recurse -Exclude "$mergeFolderName" | 
	Copy-Item -Destination {Join-Path $mergeFolder $_.FullName.Substring($targetDir.length)}


###  Copy the built plugin assembly and library files to temporary folder
###  SDK dlls that are on the CRM server should be libraries rather than ILMerged in

$toMove = @($targetFile, "$targetName.pdb", "Microsoft.Crm.Sdk.Proxy.dll", "Microsoft.Xrm.Sdk.dll", "Microsoft.Xrm.Sdk.Workflow.dll", "Microsoft.IdentityModel.dll")

$toMove | ForEach-Object {
	$copySource = "$mergeFolder\$_"
	$copyDestination = "$libFolder\$_"
	if (Test-Path $copySource)
	{
		New-Item -ItemType File -Path $copyDestination -Force
		Move-Item $copySource $copyDestination -force -Verbose
	}
}


###  Make sure that any references that need to be ILMerged in are marked as Copy Local
$ilmergeExe = $buildFolder + "\ilmerge.exe"
& $ilmergeExe /wildcards /lib:"$libFolder" /keyfile:"$buildFolder\Key.snk" /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2" /out:"$mergeFolder\$targetFile" "$libFolder\$targetFile"  "$mergeFolder\*.dll"
Write-Host `n


if (Test-Path "_POWERSHELLNOTENABLED.txt")
{
	Remove-Item "_POWERSHELLNOTENABLED.txt"
}

if ($LASTEXITCODE -ne 0) {
	exit $LASTEXITCODE
}

# Error handling
} catch {
	Write-Host `n
	exit 1
}

# SIG # Begin signature block
# MIIR2AYJKoZIhvcNAQcCoIIRyTCCEcUCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUjI7Tp+3yKKubGHZMnyK3CqMp
# oL+ggg8uMIIHWjCCBkKgAwIBAgIKWTseDwACABJWeTANBgkqhkiG9w0BAQUFADBl
# MRMwEQYKCZImiZPyLGQBGRYDbXNkMRgwFgYKCZImiZPyLGQBGRYIaW50cmFuZXQx
# EjAQBgoJkiaJk/IsZAEZFgJuYTEgMB4GA1UEAxMXQWx0aWNvci1Db3JwLUlzc3Vp
# bmctQ0EwHhcNMTUwMTA4MTkzNDMyWhcNMTcwMTA3MTkzNDMyWjCBoTELMAkGA1UE
# BhMCVVMxETAPBgNVBAgTCE1pY2hpZ2FuMQwwCgYDVQQHEwNBZGExDjAMBgNVBAoT
# BUFtd2F5MR8wHQYDVQQLExZXZWIgVGVjaG5vbG9neSBTdXBwb3J0MR8wHQYDVQQD
# ExZXZWIgVGVjaG5vbG9neSBTdXBwb3J0MR8wHQYJKoZIhvcNAQkBFhB3ZWJvcHNA
# YW13YXkuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqR0VE2WB
# WycLavK1VKGzNvxF1p3f/NSetrIuM6P0coYOLNQriiQrxydJ+0eb/ZTevLKqa5dM
# eZKnoFoMomc75N8ZFSqVO/vvh4GKHiEXELKHq7akgKPQxpBnPmXOZoZ0SN3ZYIwj
# rxOVO+HpvMoMmeHTkt8WJLaxgT1dTUE1FK8MoPdEr6hbdJ0OwuEYa4U9LlgZlGF5
# vsEIjHq1En5AIMuY7YJvqNZiPhf9XHE/TyzZx3Nkdk9Qjq2nx5ddAq+ICwDMWFyH
# 6WYoftyQAjcLtW2QZr2fadkwvb9YvncSRpiCEywL0A+vEK1RT11qZDaxKVFVes4X
# pQtZ6PM1oqv+OQIDAQABo4IDzTCCA8kwHQYDVR0OBBYEFP0sHNoF5iuxmD2ARSl/
# tqYwFBo1MB8GA1UdIwQYMBaAFHy/4b3ofHEUD36dw6Krxx4yCv3XMIIBYQYDVR0f
# BIIBWDCCAVQwggFQoIIBTKCCAUiGgb5sZGFwOi8vL0NOPUFsdGljb3ItQ29ycC1J
# c3N1aW5nLUNBLENOPVVTUEswMixDTj1DRFAsQ049UHVibGljJTIwS2V5JTIwU2Vy
# dmljZXMsQ049U2VydmljZXMsQ049Q29uZmlndXJhdGlvbixEQz1pbnRyYW5ldCxE
# Qz1tc2Q/Y2VydGlmaWNhdGVSZXZvY2F0aW9uTGlzdD9iYXNlP29iamVjdENsYXNz
# PWNSTERpc3RyaWJ1dGlvblBvaW50hj9odHRwOi8vcGtpLm5hLmludHJhbmV0Lm1z
# ZC9DZXJ0RGF0YS9BbHRpY29yLUNvcnAtSXNzdWluZy1DQS5jcmyGRGh0dHA6Ly91
# c3BrMDIubmEuaW50cmFuZXQubXNkL0NlcnRFbnJvbGwvQWx0aWNvci1Db3JwLUlz
# c3VpbmctQ0EuY3JsMIIBnwYIKwYBBQUHAQEEggGRMIIBjTCBtwYIKwYBBQUHMAKG
# gapsZGFwOi8vL0NOPUFsdGljb3ItQ29ycC1Jc3N1aW5nLUNBLENOPUFJQSxDTj1Q
# dWJsaWMlMjBLZXklMjBTZXJ2aWNlcyxDTj1TZXJ2aWNlcyxDTj1Db25maWd1cmF0
# aW9uLERDPWludHJhbmV0LERDPW1zZD9jQUNlcnRpZmljYXRlP2Jhc2U/b2JqZWN0
# Q2xhc3M9Y2VydGlmaWNhdGlvbkF1dGhvcml0eTBlBggrBgEFBQcwAoZZaHR0cDov
# L3BraS5uYS5pbnRyYW5ldC5tc2QvY2VydGRhdGEvVVNQSzAyLm5hLmludHJhbmV0
# Lm1zZF9BbHRpY29yLUNvcnAtSXNzdWluZy1DQSgyKS5jcnQwagYIKwYBBQUHMAKG
# Xmh0dHA6Ly91c3BrMDIubmEuaW50cmFuZXQubXNkL0NlcnRFbnJvbGwvVVNQSzAy
# Lm5hLmludHJhbmV0Lm1zZF9BbHRpY29yLUNvcnAtSXNzdWluZy1DQSgyKS5jcnQw
# CwYDVR0PBAQDAgeAMEAGCSsGAQQBgjcVBwQzMDEGKSsGAQQBgjcVCIeRhw6H4sFC
# hJmBKYSYyGOCh7hMgRuI8MrtbYrtoL8vAgFkAgEQMBMGA1UdJQQMMAoGCCsGAQUF
# BwMDMBsGCSsGAQQBgjcVCgQOMAwwCgYIKwYBBQUHAwMwDQYJKoZIhvcNAQEFBQAD
# ggEBAFfWoJ7LPMAfuw9N6znKEsmLL2WvFsddMoKJuUDHxlVlObQGlJHspS4Ygpp0
# l/nzu/nOxZ/OUSmVITVGYuxOnayjMJsyw9flEJ0XGWtm5xByt8/ZBJaWBtXDlRnG
# oR7z7+nnnaXH8Z8NDIgRJnMvfU7FstxcyCc8OgEE6wFzGRhF8z1qUJxAJ1F3piiy
# OzIUCMTlNSFWPEqCdvyhzPCRXJCmq3JX6eF72jqZKa4EKgeHFPxgd/RNCnSAjDGu
# yTGI9wVXXIVjdK/EXd9e3eZghJbVgYbwQh0sqo68GrvNrkkvk1WAC3BoV4zMafhR
# UPBPvyrIGGLDZCE2BK5RsBbz+dAwggfMMIIFtKADAgECAgoXLjojAAAAAAAFMA0G
# CSqGSIb3DQEBBQUAMEkxEzARBgoJkiaJk/IsZAEZFgNtc2QxGDAWBgoJkiaJk/Is
# ZAEZFghpbnRyYW5ldDEYMBYGA1UEAxMPQWx0aWNvci1Sb290LUNBMB4XDTEzMDEx
# NjE4MjUxNVoXDTIzMDExNjE4MzUxNVowZTETMBEGCgmSJomT8ixkARkWA21zZDEY
# MBYGCgmSJomT8ixkARkWCGludHJhbmV0MRIwEAYKCZImiZPyLGQBGRYCbmExIDAe
# BgNVBAMTF0FsdGljb3ItQ29ycC1Jc3N1aW5nLUNBMIIBIjANBgkqhkiG9w0BAQEF
# AAOCAQ8AMIIBCgKCAQEAplTucso9VAOlhcS4nEVVcbplfoESuXmkRCfvu9A2ULBY
# L2hwsQvdun+mFg3OVnfVCAni/6tsek34v7TcdIQPn+Vswq0qQSUodKGLj4uxDiWz
# HtbVIT2lD1fpPvSFHw2kFcQnfNv0y4KsvJ+QSz89drI0YiVdUUmAwUXXTTBvuhws
# cwfPxtj1j42823YDyG/fUF1+RxbT0RpoJuPtftRVzng1sByX4zaZGHc6zOYUyFRx
# MKfBTh+Yi+RxJ+rEap3OrZcus2VY4CaonUYmEb7x1vLWFyJeM4kBfdP1HxEw5zPt
# /Z+AmvvVOBngBuu8vIG4tHIa1WzHLNzqur1rS9cSKwIDAQABo4IDmDCCA5QwEAYJ
# KwYBBAGCNxUBBAMCAQIwIwYJKwYBBAGCNxUCBBYEFMK9S5ykgVlTlT0HpVAsFXlK
# A+6XMB0GA1UdDgQWBBR8v+G96HxxFA9+ncOiq8ceMgr91zCBxAYDVR0gBIG8MIG5
# MIG2BgkqAwQFBgcICQ4wgagwbgYIKwYBBQUHAgIwYh5gAEEAbAB0AGkAYwBvAHIA
# IABDAG8AcgBwAG8AcgBhAHQAZQAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZQAgAFAA
# cgBhAGMAdABpAGMAZQAgAFMAdABhAHQAZQBtAGUAbgB0MDYGCCsGAQUFBwIBFipo
# dHRwOi8vcGtpLm5hLmludHJhbmV0Lm1zZC9jcHMvcm9vdGNwcy5hc3AwGQYJKwYB
# BAGCNxQCBAweCgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMB
# Af8wHwYDVR0jBBgwFoAUjTZzJGd+8oto30PYWcaxodEPYQIwggEGBgNVHR8Egf4w
# gfswgfiggfWggfKGgbZsZGFwOi8vL0NOPUFsdGljb3ItUm9vdC1DQSxDTj1VU1BL
# MDEsQ049Q0RQLENOPVB1YmxpYyUyMEtleSUyMFNlcnZpY2VzLENOPVNlcnZpY2Vz
# LENOPUNvbmZpZ3VyYXRpb24sREM9SW50cmFuZXQsREM9TVNEP2NlcnRpZmljYXRl
# UmV2b2NhdGlvbkxpc3Q/YmFzZT9vYmplY3RDbGFzcz1jUkxEaXN0cmlidXRpb25Q
# b2ludIY3aHR0cDovL3BraS5uYS5pbnRyYW5ldC5tc2QvQ2VydERhdGEvQWx0aWNv
# ci1Sb290LUNBLmNybDCCAQ8GCCsGAQUFBwEBBIIBATCB/jCBrwYIKwYBBQUHMAKG
# gaJsZGFwOi8vL0NOPUFsdGljb3ItUm9vdC1DQSxDTj1BSUEsQ049UHVibGljJTIw
# S2V5JTIwU2VydmljZXMsQ049U2VydmljZXMsQ049Q29uZmlndXJhdGlvbixEQz1J
# bnRyYW5ldCxEQz1NU0Q/Y0FDZXJ0aWZpY2F0ZT9iYXNlP29iamVjdENsYXNzPWNl
# cnRpZmljYXRpb25BdXRob3JpdHkwSgYIKwYBBQUHMAKGPmh0dHA6Ly9wa2kubmEu
# aW50cmFuZXQubXNkL0NlcnREYXRhL1VTUEswMV9BbHRpY29yLVJvb3QtQ0EuY3J0
# MA0GCSqGSIb3DQEBBQUAA4ICAQAjWwEbAu2AbS+RRWyUPeyWhMx8eN4qgN9cJNcS
# WmftBIhAZTa0n7zZlr27IReCUjudQ9Bnq5SdhGtlqC5ew6fkAjqH9aa7zVILtSQL
# n2xiuwO99kGn5cwbsHU+VcgJf2i8scX8YXrT+B2RrTgGi8SY3d4TD5ze3BJ+RK2A
# rBoA5YLxrZHGvPjjrxUW6Rr67WyIH/O1W0bTVcH24BRlyDfFiMowBAZ5sTHHnz0a
# 4SHJh7Sl3BIrhw3I/LZNHuFMpUrgDuyO+zGTgp7JdqSIGx2GmdyC/NaQ24yPTY+N
# ILnu4ytS+FyTfYwdt7RPzaJSWUvjIAAVw6jpxViRwgwiMs0lKe5It2ZMuhOLaHfa
# /i57fVlDVwzcJXsN6kIqAcoyzyGAi2GSMgaHUX4m/hRcDW7ONKz3G0CFsOiEo0iB
# wlIR/houEJAGcVBtP9vrTZM5UkAx4TXQ3ZFQ6dIl2neGo2xf5tomwdDqVJjkNszz
# PeTYPcXSWD6M4M4VBm54QbKTUWjQb5fxK3UkamSH+4lQ7g48QVktUD1rGsSmzhBv
# TR8qA3H9SGqBEIdC6Xl7dqKUr3KCKwu02/1KhjKQTypzfLHac3hPhLK4AxGAx+nK
# 7pdRaZrccQy7l07FDytlLEoMBGPW6f4KJoPWTfntInmZNjfjau+7t8K3zF/wJueg
# V7XELTGCAhQwggIQAgEBMHMwZTETMBEGCgmSJomT8ixkARkWA21zZDEYMBYGCgmS
# JomT8ixkARkWCGludHJhbmV0MRIwEAYKCZImiZPyLGQBGRYCbmExIDAeBgNVBAMT
# F0FsdGljb3ItQ29ycC1Jc3N1aW5nLUNBAgpZOx4PAAIAElZ5MAkGBSsOAwIaBQCg
# eDAYBgorBgEEAYI3AgEMMQowCKACgAChAoAAMBkGCSqGSIb3DQEJAzEMBgorBgEE
# AYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMCMGCSqGSIb3DQEJ
# BDEWBBT5Aqdly/k/VNst8TP3Qb8X1l8CODANBgkqhkiG9w0BAQEFAASCAQA89w5f
# 01bfpsCj2EfSGXFw1otzOst9MiBYB/U8xHUxRADBEn/Gm1pWvumqK44TQVSqMuTe
# GiSaAb10ltKSYawb09t3Fhco3tKC7U7Ey61TA9JvWr+AbbWlBUlalTqv3b/eH62f
# ormU7Wl+Sa+Yys5FykNylliZnezng9FGWWdGKrcKwB9opJlaanXIdmDRzohh8Gd6
# 3wY5+C8I4XdQ+1M7wAd4r1oNQamE5edHJ8+8vBe+aMlOME3DvvBB2WEcjKDlADSh
# qBiEm2Vf45SP5w3tE84DRIez1YEncAkP0R1CkxXxrdbJuseRMVKjfBNcfSyJSfES
# Vcd4Lk4eTP09LUm0
# SIG # End signature block
