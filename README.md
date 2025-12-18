# 📅 CP Contest Calendar Sync

A cross-platform automation tool built with **.NET (C#)** that syncs competitive programming contests (Codeforces, LeetCode, AtCoder, etc.) directly to your Google Calendar.

![GitHub release (latest by date)](https://img.shields.io/github/v/release/Omar-Abdo1/CP-Contest-Calendar)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-blue)

## 🚀 Features
* **Auto-Sync:** Fetches upcoming contests from [Clist.by](https://clist.by/).
* **Smart Deduplication:** Prevents duplicate events even if the script runs multiple times.
* **Cross-Platform:** Runs natively on Windows and Linux.
* **Custom Alerts:** Sets reminders 1 day, 5 hours, and 1 hour before contests.
* **Background Service:** Designed to run silently on startup.

---

## 🛠️ Setup Guide

### Step 1: Download & Extract
1. Go to the [Releases Page](../../releases/latest) on this repository.
2. Download the zip file matching your OS:
   * 🪟 **Windows:** `ContestCalendar-Windows.zip`
   * 🐧 **Linux:** `ContestCalendar-Linux.zip`
3. Extract the folder to a permanent location (e.g., `Documents/ContestCalendar`).

### Step 2: Get Clist API Key
1. Register or Login at [Clist.by](https://clist.by/).
2. Go to your [API Settings](https://clist.by/api/v4/doc/).
3. Copy your **Username** and **API Key**.
4. Open the file `appsettings.json` in your downloaded folder.
5. Paste your details:
   ```json
   {
     "Clist": {
    "ApiKey": "omarabdo:9b61..."
      }
   }

### 🔐 Step 3: Get Google Credentials
To keep your Google Calendar secure, you must generate your own Access Key from Google.

- A. Go to Google Cloud Console
Visit the Google Cloud Console

Click Create Project (top bar) and name it ContestBot

Click Select Project to switch to your new project

- B. Enable Google Calendar API
In the search bar at the top, type "Google Calendar API"

Click on it from the results

Click Enable

- C. Create OAuth Credentials
Navigate to Credentials (in the left menu)

Click Create Credentials → Select OAuth Client ID

- D. Configure Consent Screen (If asked)
Choose External

Enter your email address

Click Save and Continue (you can skip the other steps for now)

- E. Create The Desktop App Key
Application Type: Select Desktop App

Click Create

A popup will appear. Click Download JSON (the download icon)

- F. Place the File
Rename the downloaded file to exactly: credentials.json

Move this file into your ContestCalendar folder (The same folder where ContestCalendarCS.exe is located).


### ▶️ How to Run

-- First Run (Authentication)
- You must run the tool manually once to grant permission to your Google Calendar.

--🪟 Windows: **Double-click ContestCalendarCS.exe**.


-- 🐧 Linux: Open a terminal in the folder and run:

Bash
```text
chmod +x ContestCalendarCS
./ContestCalendarCS
```
Note: A browser window will open. If Google warns "App not verified," click Advanced → Go to App (Unsafe). This is safe because you are the developer of the app.


### 🤖 Automation (Run on Startup)
- 🪟 Windows (Task Scheduler)
 
Search for **Task Scheduler** in the Start Menu.

Click Create Basic Task → Name it "Contest Sync".

Trigger: **When I log on**.

Action: Start a program.

Browse and select ContestCalendarCS.exe.

**Critical Step**: In the box labeled "Start in (optional)", paste the full path to the folder where the .exe is located (e.g., C:\Users\Omar\Documents\ContestCalendar\).

Finish. Now it runs every time you turn on your PC!

- 🐧 Linux (Cron Job)

Open your terminal and type:
Add the following line to the bottom of the file (replace /path/to/folder with your actual path):
```bash
crontab -e
@reboot sleep 60 && cd /path/to/ContestCalendar/ && ./ContestCalendarCS >> cronlog.txt 2>&1
```
*(The sleep 60 ensures Wi-Fi is connected before the script runs.)*


### ❤️ Contributing

If you'd like to add support for more platforms or features, feel free to fork the repository and submit a Pull Request!
