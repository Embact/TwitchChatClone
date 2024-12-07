///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Event Listeners
document.addEventListener("keydown", function (e) {
    if (e.key == "Enter" && (document.activeElement === MessageInput || document.activeElement === MessageMarkup)) {
        sendMessage();
    }
});

MessageMarkup.addEventListener("click", (e) => {
    MessageWrrpaer.classList.add("border-color");
    MessageBox.classList.add("border-color");
});

MessageMarkup.addEventListener("blur", (e) => {
    MessageWrrpaer.classList.remove("border-color");
    MessageBox.classList.remove("border-color");
});

ScrollBottom.addEventListener("click", (e) => {
    ChatBox.scrollTo({ top: ChatBox.scrollHeight + 1000000 /*, behavior: "smooth" */ });
    ToggleScroller(false);
});

ChatBox.addEventListener("scrollend", (e) => {
    HandleAutoScroll()
});
document
    .querySelector(".mod-icons-area")
    .addEventListener("click", function (e) {
        if (ModIcons) {
            ModIcons = false;
            document.querySelectorAll(".toggle-mod-icons").forEach((e) => {
                e.style.display = "none";
            });
            this.classList.remove("active");
        } else {
            ModIcons = true;
            document.querySelectorAll(".toggle-mod-icons").forEach((e) => {
                e.style.display = "flex";
            });
            this.classList.add("active");
        }
    });


PinMsg.addEventListener("click", function (e) {
    if (!e.target.closest(".dropdown") && !e.target.closest("div#HidePin")) {
        let header = this.querySelector(".pin-message-header");
        let body = this.querySelector(".pin-message-body");
        let footer = this.querySelector(".pin-message-footer");
        let toggle = this.querySelector(".toggle-pin-message");
        if (body.classList.contains("unbroke-message")) {
            // Collapse
            body.classList.remove("unbroke-message");
            PinMsg.classList.remove("Collapse");
            toggle.classList.add("flip");
        } else {
            // Fold
            body.classList.add("unbroke-message");
            PinMsg.classList.add("Collapse");
            toggle.classList.remove("flip");
        };
    }

});


document.getElementById("ChatBox").addEventListener("click", function (e) {
    const target = e.target;
    if (target.closest(".chat-message")) {
        const Msg = target.closest(".chat-message");
        const userID = Msg.getAttribute("data-user");
        const username = Msg.querySelector(".chat-prefix-items span:last-child").innerText;
        const message_id = Msg.getAttribute("data-message");
        if (target.closest(".delete")) {
            connection.invoke("DeleteMessage", message_id, GetRoom()).then(response => { });
        } else if (target.closest(".timeout")) {
            connection.invoke("Timeout", GetRoom(), userID).then(response => {

            });
        } else if (target.closest(".ban")) {
            connection.invoke("Ban", GetRoom(), userID).then(response => {

            });
        } else if (target.closest(".pin-button")) {
            connection.invoke("Pin", message_id, CurrentUser).then(response => {

            });
        } else if (target.closest(".reply-button")) {
            const MsgBody = target.closest(".chat-message").querySelector(".chat-container");
            MessageInput.value = "";
            MessageMarkup.value = "";
            MessageMarkup.focus();
            ReplyMessageId = message_id;
            ReplyBox.closest(".message-outline-wrapper").classList.add("active");
            ReplyBox.innerHTML = ReplyMessageTemplate(username,MsgBody.innerHTML);
            Root.style.setProperty("--controls-height", (100 + ReplyBox.querySelector("div").clientHeight) + "px");
            if(ReplyBox.querySelector(".reply-msg")) {
                ReplyBox.querySelector(".reply-msg").remove();
            }
            ReplyBox.querySelector(".close-reply-box").addEventListener("click",CloseReplyMessage);
            MessageInput.setAttribute("placeholder", "@" + username);
            
        }
    }
});


RestorePinMsg.addEventListener("click", function (e) {
    this.classList.add("d-none");
    PinMsg.classList.remove("d-none");
});

EmojiButton.addEventListener("click",function(e) {
    if (FloatingBox.classList.contains("d-none")) {
        FloatingBox.classList.remove("d-none");
        LoadEmojies();
    } else {
        CloseFloatingBox();
    }
});

FloatingBox.addEventListener("click",function(e) {
    if(e.target.closest(".emoji")){
        var img = e.target.closest(".emoji");
        MessageInput.value = MessageInput.value + img.alt;
        MessageMarkup.appendChild(img.cloneNode(true));
    }
});



document.getElementById('messageMarkup').addEventListener('input', function() {
    MessageMarkup.querySelectorAll("br").forEach(s => s.remove());
    MessageMarkup.querySelectorAll("div").forEach(s => s.remove());
    // MessageMarkup.innerText = handleMessageText(MessageMarkup.innerText);
    
    // Replace emoji codes with image tags
    // MessageMarkup.innerText = MessageMarkup.innerText.replace(/:\w+:/g, function (match) {
    //     var imageNumber = match.replace(":e","").replace(":","");
    //     return imageNumber > 0 && imageNumber <= 200 ? `<img src="../assets/images/emojis/${imageNumber}.png"  data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="${match}" alt="${match}" class="emoji">` : match;
    // });
    
// Get the HTML content
let htmlContent = MessageMarkup.innerHTML;

// Create a temporary div to hold the HTML
const tempDiv = document.createElement('div');
tempDiv.innerHTML = htmlContent;

// Replace img.emoji elements with their alt attributes
tempDiv.querySelectorAll("img.emoji").forEach(img => {
    const altText = img.alt;
    // Replace the img with its alt text
    const span = document.createElement('span');
    span.textContent = altText;
    img.parentNode.replaceChild(span, img);
});

// Convert the updated HTML to plain text
const textContent = tempDiv.innerText || tempDiv.textContent;

// Set the result to MessageInput
MessageInput.value = textContent;
});


CommunityBtn.addEventListener('click', function() {
    if(Community.classList.contains("d-none")) {
        document.querySelector(".stream-title").innerText = "COMMUNITY";
        CommunityBtn.innerHTML = `
            <svg width="20" height="20" fill="#FFF" version="1.1" viewBox="0 0 20 20" x="0px" y="0px" role="presentation" aria-hidden="true" focusable="false" class="ScIconSVG-sc-1q25cff-1 jpczqG"><g><path d="M11 8h2v2h-2V8zM9 8H7v2h2V8z"></path><path d="M10 18l-3-3H5a2 2 0 01-2-2V5a2 2 0 012-2h10a2 2 0 012 2v8a2 2 0 01-2 2h-2l-3 3zm-2.172-5L10 15.172 12.172 13H15V5H5v8h2.828z" fill-rule="evenodd" clip-rule="evenodd"></path></g></svg>
        `;
        Community.classList.remove("d-none");
        
    } else {
        document.querySelector(".stream-title").innerText = "STREAM CHAT";
        CommunityBtn.innerHTML = `
            <svg width="20" height="20" fill="#FFF" version="1.1" viewBox="0 0 20 20" x="0px" y="0px" role="presentation" aria-hidden="true" focusable="false" class="ScIconSVG-sc-1q25cff-1 jpczqG"><g><path fill-rule="evenodd" d="M7 2a4 4 0 00-1.015 7.87c-.098.64-.651 1.13-1.318 1.13A2.667 2.667 0 002 13.667V18h2v-4.333c0-.368.298-.667.667-.667.908 0 1.732-.363 2.333-.953.601.59 1.425.953 2.333.953.369 0 .667.299.667.667V18h2v-4.333A2.667 2.667 0 009.333 11c-.667 0-1.22-.49-1.318-1.13A4.002 4.002 0 007 2zM5 6a2 2 0 104 0 2 2 0 00-4 0z" clip-rule="evenodd"></path><path d="M14 11.83V18h4v-3.75c0-.69-.56-1.25-1.25-1.25a.75.75 0 01-.75-.75v-.42a3.001 3.001 0 10-2 0z"></path></g></svg>
        `;
        Community.classList.add("d-none")
    }
});

Community.addEventListener('click', function(e) {
    if(e.target.closest(".un-ban-user")) {
        var name = e.target.closest(".un-ban-user").dataset.name;
        var options = {
            method : "post"
        }
        fetch(`${BaseUrl}/Chat/${GetRoom()}/unban?user=${name}`,options);
    } else if(e.target.closest(".ban-user")) {
        var name = e.target.closest(".ban-user").dataset.name;
        var options = {
            method : "post"
        }
        fetch(`${BaseUrl}/Chat/${GetRoom()}/ban?user=${name}`,options);
    }
});