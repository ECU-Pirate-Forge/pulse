#!/usr/bin/env python3
"""
Backlog Viewer - Display all open issues from the GitHub repository
"""

import json
import os
import sys
from urllib.request import Request, urlopen
from urllib.error import HTTPError, URLError


def fetch_issues(owner, repo, token=None):
    """Fetch all open issues from GitHub API"""
    url = f"https://api.github.com/repos/{owner}/{repo}/issues?state=open&per_page=100"
    
    headers = {
        "Accept": "application/vnd.github.v3+json",
        "User-Agent": "Backlog-Viewer"
    }
    
    if token:
        headers["Authorization"] = f"token {token}"
    
    try:
        req = Request(url, headers=headers)
        with urlopen(req) as response:
            return json.loads(response.read().decode())
    except HTTPError as e:
        print(f"HTTP Error: {e.code} - {e.reason}")
        return []
    except URLError as e:
        print(f"URL Error: {e.reason}")
        return []


def display_backlog(issues):
    """Display issues in a readable format"""
    if not issues:
        print("\n✨ No open issues found! Your backlog is empty.\n")
        return
    
    print("\n" + "=" * 80)
    print(f"📋 BACKLOG - {len(issues)} Open Issue(s)")
    print("=" * 80 + "\n")
    
    for i, issue in enumerate(issues, 1):
        # Skip pull requests (they appear as issues in the API)
        if 'pull_request' in issue:
            continue
            
        number = issue['number']
        title = issue['title']
        user = issue['user']['login']
        created = issue['created_at'][:10]
        comments = issue['comments']
        labels = [label['name'] for label in issue.get('labels', [])]
        
        print(f"{i}. Issue #{number}: {title}")
        print(f"   Created by: {user} on {created}")
        print(f"   Comments: {comments}")
        if labels:
            print(f"   Labels: {', '.join(labels)}")
        print(f"   URL: {issue['html_url']}")
        print()


def main():
    """Main function"""
    # Default repository info
    owner = "ECU-Pirate-Forge"
    repo = "pulse"
    
    # Check for GitHub token in environment
    token = os.environ.get('GITHUB_TOKEN')
    
    print("Fetching backlog...")
    issues = fetch_issues(owner, repo, token)
    display_backlog(issues)


if __name__ == "__main__":
    main()
