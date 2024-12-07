# Twitch Chat Clone  

🚀 **Real-Time Chat Application Inspired by Twitch**  

## Overview  
Twitch Chat Clone is a real-time chat application that mimics the functionality of Twitch's chat system. Built with a modern tech stack, this project demonstrates features like multi-room chat, message moderation, user roles, and dynamic message handling.  

## Tech Stack  
- **Server**: ASP.NET Core API 8.0 + SignalR  
- **Client**: Vanilla JavaScript  

## Features  
### Core Functionality  
- **Real-time Communication**: Powered by SignalR.  
- **Multi-Room Support**: Users can chat in multiple rooms.  
- **Message History**: Display old messages to new users.  

### Moderation  
- **Delete Messages**  
- **Pin/Unpin Messages**  
- **Ban/Unban Users**  
- **Timeout Users**  

### Messaging Enhancements  
- **Reply to Messages**  
- **Send Emojis**  
- **Roles**: Includes Broadcaster, Moderator, Prime Gaming, Verified, etc.  

### System Highlights  
- **API-Driven Architecture**: Handles all chat features via API and SignalR hub.  
- **Message Queue System**: Manages sending and queuing of messages for optimal performance.  
- **Background Workers**: Automates tasks like unblocking timed-out users.  
- **Dynamic Interval Management**: Adjusts message intervals based on queue size.  
- **Bulk Message Delivery**: Improves performance by sending messages in bulk instead of one by one.  
- **DOM Optimization**: Limits visible messages to 150, keeping the client clean and responsive.  

### Challenges & Future Work  
- **Ghost Messages**: Handling large message queues effectively to ensure no messages are skipped.  

### Clone the Repository  
```bash  
git clone https://github.com/Embact/TwitchChatClone.git
cd TwitchChatClone