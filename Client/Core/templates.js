function MessageTemplate(message,isOld = false) {
    var msgDate = new Date(message.date);
    const options = {
        hour: 'numeric',
        minute: 'numeric',
    };
    const timeString = new Intl.DateTimeFormat('en-US', options).format(msgDate).replace(" AM","").replace(" PM","");
    template = `
    <div class='floated-buttons'>
        ${CurrentUser.roles.includes(Badges.Broadcaster) || CurrentUser.roles.includes(Badges.Moderator) ? 
            `
            <div class='pin-button' data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Pin this message">
                <svg width="20px" height="20px" fill="#FFFFFF" viewBox="0 0 20 20" focusable="false" aria-hidden="true" role="presentation"><path fill-rule="evenodd" d="M4.941 2h10v2H13v3a3 3 0 0 1 3 3v3H4v-3a3 3 0 0 1 3-3V4H4.941V2zM9 9H7a1 1 0 0 0-1 1v1h8v-1a1 1 0 0 0-1-1h-2V4H9v5z" clip-rule="evenodd"></path><path d="M10.999 15h-2v3h2v-3z"></path></svg>
            </div>
            ` : 
            `` 
        }        
        <div class='reply-button' data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Click to reply">
            <svg width="20px" height="20px" fill="#FFFFFF" version="1.1" viewBox="0 0 20 20" x="0px" y="0px" role="presentation" aria-hidden="true" focusable="false" class="ScIconSVG-sc-1q25cff-1 jpczqG"><path d="M8.5 5.5L7 4L2 9L7 14L8.5 12.5L6 10H10C12.2091 10 14 11.7909 14 14V16H16V14C16 10.6863 13.3137 8 10 8H6L8.5 5.5Z"></path></svg>
        </div>
    </div>
    <div class='chat-container'>
        ${message.reply != null ? 
            `
            <div class='reply-msg d-flex align-items-center gap-1'>
                <div><svg width="17" height="17" fill="#ADADB8" viewBox="0 0 16 16"><path d="M5 6h2v2H5V6Zm4 0h2v2H9V6Z"></path><path fill-rule="evenodd" d="m8 14 2-2h3a1 1 0 0 0 1-1V3a1 1 0 0 0-1-1H3a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1h3l2 2Zm-1.172-4H4V4h8v6H9.172L8 11.172 6.828 10Z" clip-rule="evenodd"></path></svg></div>
                <span>Replying to @${message.reply.sender.username} : ${handleMessageText(message.reply.text)}</span>
            </div>
            `
            :``
        }
        
        <div class='chat-message-prefix'>         
            <div class='d-flex align-items-center gap-1'>
            ${
                isOld ? 
                `<span class='timestamp-color me-1'>
                    ${timeString}
                </span>` 
                : 
                ``
            }
            ${CurrentUser.roles.includes(Badges.Broadcaster) || CurrentUser.roles.includes(Badges.Moderator) ? 
                `
                <div class='mod-buttons gap-1 toggle-mod-icons' style='display:${ModIcons ? 'flex' : 'none'}'>
                    <span class='ban' data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Ban">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="16" height="16" color="#FFFFFF" fill="none">
                            <path d="M5.75 5L19.75 19" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                            <path d="M22.75 12C22.75 6.47715 18.2728 2 12.75 2C7.22715 2 2.75 6.47715 2.75 12C2.75 17.5228 7.22715 22 12.75 22C18.2728 22 22.75 17.5228 22.75 12Z" stroke="currentColor" stroke-width="1.5" />
                        </svg>
                    </span>
                    <span class='timeout' data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Timeout">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="16" height="16" color="#FFFFFF" fill="none">
                            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="1.5" />
                            <path d="M12 8V12L14 14" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        </svg>
                    </span>
                    <span class='delete' data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="16" height="16" color="#FFFFFF" fill="none">
                            <path d="M19.5 5.5L18.8803 15.5251C18.7219 18.0864 18.6428 19.3671 18.0008 20.2879C17.6833 20.7431 17.2747 21.1273 16.8007 21.416C15.8421 22 14.559 22 11.9927 22C9.42312 22 8.1383 22 7.17905 21.4149C6.7048 21.1257 6.296 20.7408 5.97868 20.2848C5.33688 19.3626 5.25945 18.0801 5.10461 15.5152L4.5 5.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
                            <path d="M3 5.5H21M16.0557 5.5L15.3731 4.09173C14.9196 3.15626 14.6928 2.68852 14.3017 2.39681C14.215 2.3321 14.1231 2.27454 14.027 2.2247C13.5939 2 13.0741 2 12.0345 2C10.9688 2 10.436 2 9.99568 2.23412C9.8981 2.28601 9.80498 2.3459 9.71729 2.41317C9.32164 2.7167 9.10063 3.20155 8.65861 4.17126L8.05292 5.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
                            <path d="M9.5 16.5L9.5 10.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
                            <path d="M14.5 16.5L14.5 10.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
                        </svg>
                    </span>
                </div>
                `: 
                `` }
                
                <div class='chat-prefix-items'>
                    ${GenerateUerBadges(message.sender.roles)}
                    <span class='fw-bold' style='color: ${message.sender.username == GetName() ? "#F00" : message.sender.color};'>${message.sender.username}</span>
                </div>
                <span>:</span>
            </div>
        </div>
        <span class='message-container'>
            <span class='message'>
                ${handleMessageText(message.text)}
            </span>
        </span>
    </div>
    `;
    return template;
}


function BanTemplate() {
    let template = `
        <div class='ban-template d-flex flex-column gap-2 text-center'>
            <div class='ban-icon'>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="60" height="60" color="#ffffff" fill="none">
                    <path d="M16 12L8 12" stroke="currentColor" stroke-width="2" />
                    <path d="M22 12C22 6.47715 17.5228 2 12 2C6.47715 2 2 6.47715 2 12C2 17.5228 6.47715 22 12 22C17.5228 22 22 17.5228 22 12Z" stroke="currentColor" stroke-width="2" />
                </svg>
            </div>
            <div>
                <b>You are banned from Chat</b>
            </div>
            <div class='px-4'>
                <p class='px-2 m-0'>You are unable to participate in this channel's chat until a moderator unbans you.This channel allows banned users to request unban once they have waited 2 months</p>
            </div>
            <div class='mt-2'>
                <a href='#' class='text-decoration-none fw-medium' style='color:#a66bff;'>View Details</a>
            </div>
        </div>
    `;
    return template;
}


function PinMessageTemplate(pinnedBy,message,messageBy,date) {
    let template = `
    <div class='pin-message-control'>
        ${CurrentUser.roles.includes(Badges.Broadcaster) || CurrentUser.roles.includes(Badges.Moderator) ? 
            `
            <div class="dropdown hidable">
                <a class="" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20" focusable="false" aria-hidden="true" role="presentation"><path d="M10 18a2 2 0 1 1 0-4 2 2 0 0 1 0 4zm0-6a2 2 0 1 1 0-4 2 2 0 0 1 0 4zM8 4a2 2 0 1 0 4 0 2 2 0 0 0-4 0z"></path></svg>
                </a>
                <ul class="dropdown-menu">
                    <li><a id='UnPin' class="dropdown-item" href="#">
                        <div class='d-flex gap-2 align-items-center'>
                            <svg width="20" height="20" fill='#FFF' version="1.1" viewBox="0 0 20 20" x="0px" y="0px"><g><path d="M14 9H6v2h8V9z"></path><path fill-rule="evenodd" d="M2 10a8 8 0 1116 0 8 8 0 01-16 0zm8 6a6 6 0 110-12 6 6 0 010 12z" clip-rule="evenodd"></path></g></svg>
                            <span>Unpin this Message</span>
                        </div>
                    </a></li>
                    <li><a id='HidePin' class="dropdown-item" href="#">
                        <div class='d-flex gap-2 align-items-center'>
                            <svg width="20" height="20" fill='#FFF' version="1.1" viewBox="0 0 20 20" x="0px" y="0px" class="ScIconSVG-sc-1q25cff-1 jpczqG"><g><path fill-rule="evenodd" d="M16.5 18l1.5-1.5-2.876-2.876a9.99 9.99 0 001.051-1.191L18 10l-1.825-2.433a9.992 9.992 0 00-2.855-2.575 35.993 35.993 0 01-.232-.14 6 6 0 00-6.175 0 35.993 35.993 0 01-.35.211L3.5 2 2 3.5 16.5 18zm-2.79-5.79a8 8 0 00.865-.977L15.5 10l-.924-1.233a7.996 7.996 0 00-2.281-2.058 37.22 37.22 0 01-.24-.144 4 4 0 00-4.034-.044l1.53 1.53a2 2 0 012.397 2.397l1.762 1.762z" clip-rule="evenodd"></path><path d="M11.35 15.85l-1.883-1.883a3.996 3.996 0 01-1.522-.532 38.552 38.552 0 00-.239-.144 7.994 7.994 0 01-2.28-2.058L4.5 10l.428-.571L3.5 8 2 10l1.825 2.433a9.992 9.992 0 002.855 2.575c.077.045.155.092.233.14a6 6 0 004.437.702z"></path></g></svg>
                            <span>Hide for Yourself</span>
                        </div>
                    </a></li>
                </ul>
            </div>
            `
            :
            `
            <div id='HidePin' class="toggle-pin-message hidable">
                <svg width="20" height="20" fill='#FFF' version="1.1" viewBox="0 0 20 20" x="0px" y="0px" class="ScIconSVG-sc-1q25cff-1 jpczqG"><g><path fill-rule="evenodd" d="M16.5 18l1.5-1.5-2.876-2.876a9.99 9.99 0 001.051-1.191L18 10l-1.825-2.433a9.992 9.992 0 00-2.855-2.575 35.993 35.993 0 01-.232-.14 6 6 0 00-6.175 0 35.993 35.993 0 01-.35.211L3.5 2 2 3.5 16.5 18zm-2.79-5.79a8 8 0 00.865-.977L15.5 10l-.924-1.233a7.996 7.996 0 00-2.281-2.058 37.22 37.22 0 01-.24-.144 4 4 0 00-4.034-.044l1.53 1.53a2 2 0 012.397 2.397l1.762 1.762z" clip-rule="evenodd"></path><path d="M11.35 15.85l-1.883-1.883a3.996 3.996 0 01-1.522-.532 38.552 38.552 0 00-.239-.144 7.994 7.994 0 01-2.28-2.058L4.5 10l.428-.571L3.5 8 2 10l1.825 2.433a9.992 9.992 0 002.855 2.575c.077.045.155.092.233.14a6 6 0 004.437.702z"></path></g></svg>
            </div>
            `  
        }


        <div class="toggle-pin-message">
            <svg width="20" height="20" fill="#FFF" version="1.1" viewBox="0 0 20 20" x="0px" y="0px" role="presentation" aria-hidden="true" focusable="false"><g><path d="M14.5 6.5L10 11 5.5 6.5 4 8l6 6 6-6-1.5-1.5z"></path></g></svg>
        </div>
    </div>
    <div class='pin-wrapper'>
        <div class="pin-message-header d-flex align-items-center">
            <svg width="16" height="16" fill="#FFF" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4.941 2h10v2H13v3a3 3 0 0 1 3 3v3H4v-3a3 3 0 0 1 3-3V4H4.941V2zM9 9H7a1 1 0 0 0-1 1v1h8v-1a1 1 0 0 0-1-1h-2V4H9v5z" clip-rule="evenodd"></path><path d="M10.999 15h-2v3h2v-3z"></path></svg>
            <div class="px-1">
                <span class='pinned-by'>Pinned by ${pinnedBy}</span>
            </div>
        </div>
        <div class="pin-message-body">
            <span>${message}</span>
        </div>
        <div class="pin-message-footer">
            <span class='d-flex align-items-center gap-1'>${messageBy} <span style='margin-bottom:-3px;'><span style='font-size:12px;'>sent at</span> ${date}</span></span>
        </div>
    </div>
    `;
    return template;
}


function ReplyMessageTemplate(username,messageContnet) {
    let template = `
        <div class="p-2 pb-1">
            <div class="d-flex align-items-center gap-1">
                <svg width="20px" height="20px" fill="#FFF" version="1.1" viewBox="0 0 20 20" x="0px" y="0px" class="ScSvg-sc-1hrsqw6-1 ihOSMR"><path d="M8.5 5.5L7 4L2 9L7 14L8.5 12.5L6 10H10C12.2091 10 14 11.7909 14 14V16H16V14C16 10.6863 13.3137 8 10 8H6L8.5 5.5Z"></path></svg>
                <span class="flex-grow-1 text-white">Replying to @${username}</span>
                <button class="btn-embact close-reply-box" aria-label="Close" title="Close"><svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20" focusable="false" aria-hidden="true" role="presentation"><path d="M8.5 10 4 5.5 5.5 4 10 8.5 14.5 4 16 5.5 11.5 10l4.5 4.5-1.5 1.5-4.5-4.5L5.5 16 4 14.5 8.5 10z"></path></svg></button>
            </div>
            <div>
                ${messageContnet}
            </div>
        </div>
    `;
    return template;
}

function EmojiBoxTemaplte() {
    let template = `
        <div class="emoji-box-header d-flex justify-content-between align-items-center">
                <button class="btn-embact ms-auto close-emoji-box" aria-label="Close" title="Close"><svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20" focusable="false" aria-hidden="true" role="presentation"><path d="M8.5 10 4 5.5 5.5 4 10 8.5 14.5 4 16 5.5 11.5 10l4.5 4.5-1.5 1.5-4.5-4.5L5.5 16 4 14.5 8.5 10z"></path></svg></button>
        </div>
        <div class="emoji-box-body"></div>
    `;
    return template;
}

function CommunityTemplate() {
    let template = "";
    
    // Start .accordion
    template += `
        <div class="accordion" id="accordionPanelsStayOpenExample">
    `;


    AllUsers.forEach(group => {   
        if (group[0].role != -1)     
            template += CommunityItemTemplate(GetBadgeByValue(group[0].role),group);
        
    })

    if (AllUsers.filter(s => s[0].role == -1).length > 0)     
        template += CommunityItemTemplate("Viewers",AllUsers.filter(s => s[0].role == -1)[0]);
    

    // End .accordion
    template += `
        </div>
    `;
    return template;
}


function CommunityItemTemplate(title,users) {
    let template = ``;

    template = `
        <div class="accordion-item">
            <h2 class="accordion-header">
                <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#TAB_${title}" aria-expanded="true" aria-controls="TAB_${title}">
                    <div class="d-flex justify-content-between w-100 pe-2">
                        <div class="d-flex gap-2 align-items-center">
                            <span>
                                ${title == "Viewers" ? 
                                    `
                                    <svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M7 2a4 4 0 0 0-1.015 7.87A1.334 1.334 0 0 1 4.667 11 2.667 2.667 0 0 0 2 13.667V18h2v-4.333c0-.368.298-.667.667-.667A3.32 3.32 0 0 0 7 12.047 3.32 3.32 0 0 0 9.333 13c.369 0 .667.299.667.667V18h2v-4.333A2.667 2.667 0 0 0 9.333 11c-.667 0-1.22-.49-1.318-1.13A4.002 4.002 0 0 0 7 2zM5 6a2 2 0 1 0 4 0 2 2 0 0 0-4 0z" clip-rule="evenodd"></path><path d="M14 11.83V18h4v-3.75c0-.69-.56-1.25-1.25-1.25a.75.75 0 0 1-.75-.75v-.42a3.001 3.001 0 1 0-2 0z"></path></svg>
                                    `
                                    :
                                    `
                                    <img src="../../assets/images/roles/${title}.png" width="20" height="20">
                                    `
                                }
                                
                            </span>
                            <span style="margin-top: 3px;">${title}</span>
                        </div>
                        <span><svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20" aria-label="The host of this channel, serving you the freshest content."><path fill-rule="evenodd" d="M10 18a8 8 0 1 0 0-16 8 8 0 0 0 0 16zM9 8V6h2v2H9zm0 6V9h2v5H9z" clip-rule="evenodd"></path></svg></span>
                    </div>
                </button>
            </h2>
            <div id="TAB_${title}" class="accordion-collapse collapse show">
                <div class="accordion-body d-flex flex-column gap-2">`
                users.forEach(info => {
                    var userInfo = info.userInfo;
                    template += `
                        <div class="user-info d-flex align-items-center justify-content-between">
                            <span class='d-flex align-items-center gap-2'>
                                ${userInfo.user.username}
                                ${ userInfo.isBanned ? 
                                    `
                                    <span class="badge embact-badge px-1">Banned</span>
                                    `
                                    :
                                    ``
                                }
                            </span>
                            <div class="d-flex gap-1">
                                ${ userInfo.isBanned && userInfo.user.username != CurrentUser.username && (CurrentUser.roles.includes(Badges.Broadcaster) || CurrentUser.roles.includes(Badges.Moderator)) ? 
                                    `
                                    <button class="btn-embact un-ban-user" data-name='${userInfo.user.username}'>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="20" height="20" color="#18d321" fill="none">
                                            <path d="M4.26781 18.8447C4.49269 20.515 5.87613 21.8235 7.55966 21.9009C8.97627 21.966 10.4153 22 12 22C13.5847 22 15.0237 21.966 16.4403 21.9009C18.1239 21.8235 19.5073 20.515 19.7322 18.8447C19.879 17.7547 20 16.6376 20 15.5C20 14.3624 19.879 13.2453 19.7322 12.1553C19.5073 10.485 18.1239 9.17649 16.4403 9.09909C15.0237 9.03397 13.5847 9 12 9C10.4153 9 8.97627 9.03397 7.55966 9.09909C5.87613 9.17649 4.49269 10.485 4.26781 12.1553C4.12104 13.2453 4 14.3624 4 15.5C4 16.6376 4.12104 17.7547 4.26781 18.8447Z" stroke="currentColor" stroke-width="2" />
                                            <path d="M7.5 9V6.5C7.5 4.01472 9.51472 2 12 2C13.9593 2 15.5 3.5 16 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                                            <path d="M11.9961 15.5H12.0051" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                                        </svg>
                                    </button>
                                    `
                                    :
                                    ``
                                }
                                ${ !userInfo.isBanned && userInfo.user.username != CurrentUser.username && (CurrentUser.roles.includes(Badges.Broadcaster) || CurrentUser.roles.includes(Badges.Moderator)) ? 
                                    `
                                    <button class="btn-embact ban-user" data-name='${userInfo.user.username}'>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="20" height="20" color="#d31818" fill="none">
                                            <path d="M12 16.5V14.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
                                            <path d="M4.26781 18.8447C4.49269 20.515 5.87613 21.8235 7.55966 21.9009C8.97627 21.966 10.4153 22 12 22C13.5847 22 15.0237 21.966 16.4403 21.9009C18.1239 21.8235 19.5073 20.515 19.7322 18.8447C19.879 17.7547 20 16.6376 20 15.5C20 14.3624 19.879 13.2453 19.7322 12.1553C19.5073 10.485 18.1239 9.17649 16.4403 9.09909C15.0237 9.03397 13.5847 9 12 9C10.4153 9 8.97627 9.03397 7.55966 9.09909C5.87613 9.17649 4.49269 10.485 4.26781 12.1553C4.12104 13.2453 4 14.3624 4 15.5C4 16.6376 4.12104 17.7547 4.26781 18.8447Z" stroke="currentColor" stroke-width="1.5" />
                                            <path d="M7.5 9V6.5C7.5 4.01472 9.51472 2 12 2C14.4853 2 16.5 4.01472 16.5 6.5V9" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                                        </svg>
                                    </button>
                                    `
                                    :
                                    ``
                                }
                                <button class="btn-embact">
                                    <svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20" focusable="false" aria-hidden="true" role="presentation">
                                        <path d="M9 6h2v3H9V6zm0 5a1 1 0 1 1 2 0 1 1 0 0 1-2 0z"></path>
                                        <path fill-rule="evenodd"
                                            d="m7 15 3 3 3-3h2a2 2 0 0 0 2-2V5a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h2zm3 .172L7.828 13H5V5h10v8h-2.828L10 15.172z"
                                            clip-rule="evenodd"></path>
                                    </svg>
                                </button>
                                <button class="btn-embact">
                                    <svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20"
                                        focusable="false" aria-hidden="true" role="presentation">
                                        <path fill-rule="evenodd"
                                            d="M7.828 13 10 15.172 12.172 13H15V5H5v8h2.828zM10 18l-3-3H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2h-2l-3 3z"
                                            clip-rule="evenodd"></path>
                                    </svg>
                                </button>
                                <button class="btn-embact">
                                    <svg width="20" height="20" fill="#FFF" viewBox="0 0 20 20"
                                        focusable="false" aria-hidden="true" role="presentation">
                                        <path fill-rule="evenodd"
                                            d="M16 6h2v6h-1v6H3v-6H2V6h2V4.793c0-2.507 3.03-3.762 4.803-1.99.131.131.249.275.352.429L10 4.5l.845-1.268a2.81 2.81 0 0 1 .352-.429C12.969 1.031 16 2.286 16 4.793V6zM6 4.793V6h2.596L7.49 4.341A.814.814 0 0 0 6 4.793zm8 0V6h-2.596l1.106-1.659a.814.814 0 0 1 1.49.451zM16 8v2h-5V8h5zm-1 8v-4h-4v4h4zM9 8v2H4V8h5zm0 4H5v4h4v-4z"
                                            clip-rule="evenodd"></path>
                                    </svg>
                                </button>
                            </div>
                        </div>
                    `
                });

    template += `                    
                </div>
            </div>
        </div>
    `;

    return template;
}