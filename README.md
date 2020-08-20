# WMI Disk Monitor Service

This is a project i did a few year ago, it's a service for Windows Operating System that uses WMI (Windows Management Instrumentaion) to monitor disk space on a list of Windows Machines that is saved in a SQL Server database.

It creates a table for each Windows Machine that monitors to keep a record of how the space has been evolving and sends mails when disk space has passed a certain treshold.

## Install

To install this service i used the next command:

> installutil MonitoreoDiscosService.exe

Or you could also try with powershell:

> New-Service -Name "My Super Cool Disk Monitor" -BinaryPathName MonitoreoDiscosService.exe

You have to check the user running the service. It sould be a user that has rights to run WMI Queries on the remote Windows Machines.

# Uninstall

To uninstall this service i used the next command:

> installutil /u MonitoreoDiscosService.exe

Or you could alsy try with powershell:

> Remove-Service -Name "My Disk Monitor"
>
> sc.exe delete "My Super Cool Monitor"
