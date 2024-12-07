///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Variables
let BaseUrl          = "https://localhost:7179/api/v1";
let Username         = document.querySelector(".chat-user");
let ChatBox          = document.getElementById("ChatBox");
var MessageWrrpaer   = document.querySelector(".message-wrapper");
var MessageBox       = document.querySelector(".message-box");
let MessageInput     = document.getElementById("msgInput");
let MessageMarkup    = document.getElementById("messageMarkup");
let ScrollBottom     = document.querySelector(".scroll-to-bottom");
let ConnectionTab    = document.querySelector(".connection-tab");
let ConnectionText   = document.getElementById("connectionText");
let PinMsg           = document.querySelector(".pin-message");
let RestorePinMsg    = document.querySelector("#RestorePin");
let ReplyBox         = document.querySelector(".reply-box-wrapper");
let FloatingBox      = document.querySelector(".floating-box");
let EmojiButton      = document.querySelector("#EmojiButton");
let CommunityBtn     = document.querySelector("#Community");
let Community        = document.querySelector(".community");
var Root             = document.querySelector(':root');
let retryCount       = 0;
let MaxRetry         = 5;
let ModIcons         = false;
let isScrollAtEnd    = true;
let TimeOut          = null;
let TimeOutSeconds   = 15;
let shouldReconnect  = false;
let IsDisconnected   = false;
let CurrentUser      = null;
let ReplyMessageId   = null;
let msgScrollTimeOut = null;
let AllUsers         = [];

let wallpapers = [
  "#00ae97",
  "#ed3b3b",
  "#edde3b",
  "#3b97ed",
  "#ed7c3b",
  "#c03bed",
  "#703bed",
  "#4aba8b",
  "#8fba4a",
  "#4a75ba",
  "#9b47ef",
  "#68c5d8",
  "#6cd80d",
  "#6c5b99",
  "#5b9996",
  "#99705b",
  "#5e995b",
  "#865b99",
  "#00f2ff",
  "#65ff00",
  "#ffd400",
  "#ff6100",
  "#005dff",
  "#e100ff",
];

document.querySelector(".wallpaper").style.background = `${
  wallpapers[Math.floor(Math.random() * wallpapers.length)]
}`;
document.querySelector(".wallpaper").style.backgroundSize = `5%`;
document.querySelector(".wallpaper").style.backgroundPosition = `0 0`;

// let mouseX = 0;
// let mouseY = 0;
// let targetX = 0;
// let targetY = 0;

// document.addEventListener("mousemove", function (e) {
//   targetX = (e.clientX / window.innerWidth) * 100;
//   targetY = (e.clientY / window.innerHeight) * 100;
// });

// function animate() {
//   mouseX += (targetX - mouseX) * 0.1;
//   mouseY += (targetY - mouseY) * 0.1;

//   document
//     .querySelector(".wallpaper")
//     .style.setProperty("--x", `-${mouseX / 3}%`);
//   document
//     .querySelector(".wallpaper")
//     .style.setProperty("--y", `-${mouseY / 3}%`);

//   requestAnimationFrame(animate);
// }

// animate();
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Objects

function User(name, room = null, id = null, color = null, roles = []) {
  let user = {
    id: id,
    userName: name,
    chatRoom: room,
    color: color,
    roles: roles,
  };
  return user;
}

function Message(userObj, msg) {
  let message = {
    sender: userObj,
    text: msg,
    date: null,

  };
  return message;
}

function GetRandomColor() {
  const colors = ["Red", "Blue", "Green", "Yellow", "Purple"];
  return colors[Math.floor(Math.random() * colors.length)];
}

const UserColor = GetRandomColor();
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Methods
let Badges = {
  Broadcaster: 0,
  Moderator: 1,
  Vip: 2,
  Prime: 3,
  Verified: 4,
};

function GetBadgeByValue(value) {
  return Object.keys(Badges).find(key => Badges[key] === value);
}

function GetName() {
  const params = new Proxy(new URLSearchParams(window.location.search), {
    get: (searchParams, prop) => searchParams.get(prop),
  });
  return params.name ?? "Anonymous";
}

function GetRoom() {
  const params = new Proxy(new URLSearchParams(window.location.search), {
    get: (searchParams, prop) => searchParams.get(prop),
  });
  return params.room ?? "Anonymous";
}

function GetBadge(badge, userBadges = []) {
  if (userBadges.includes(badge)) {
    return true;
  }
  return false;
}

function UserBadges() {
  const params = new Proxy(new URLSearchParams(window.location.search), {
    get: (searchParams, prop) => searchParams.get(prop),
  });
  if (params.badges != null) {
    let badges = params.badges.split(",").map((s) => parseInt(s));
    return badges;
  }
  return [];
}
function HandleAutoScroll() {
  if (ChatBox.scrollTop + 50 >= ChatBox.scrollHeight - ChatBox.clientHeight) {
    isScrollAtEnd = true;
    ChatBox.scrollTo({ top: ChatBox.scrollHeight, behavior: "smooth" });
    ToggleScroller(false);
  } else {
    isScrollAtEnd = false;
    ToggleScroller();
  }
}
function ToggleScroller(show = true) {
  if (show && (ChatBox.scrollHeight - ChatBox.clientHeight) > 0) {
    ScrollBottom.classList.remove("d-none");
  } else {
    ScrollBottom.classList.add("d-none");
  }
}

function handleMessageText(text) {
  // Regular expression to match URLs
  const urlPattern =
    /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gi;

  // Regular expression to match mentions (e.g., @username)
  const mentionPattern = /(@\w+)/g;

  // Replace URLs with anchor tags
  text = text.replace(urlPattern, function (url) {
    return `<span class='message-url'><a href="${url}" target="_blank">${url}</a></span>`;
  });

  // Replace mentions with span tags
  text = text.replace(mentionPattern, function (mention) {
    return `<span class='mention' ${CurrentUser.username == mention.replace("@","") ? `style='background-color:#FFF;color:#000;border-radius:2px;'` : ``} >${mention}</span>`;
  });

    // Replace emoji codes with image tags
  text = text.replace(/:\w+:/g, function (match) {
    var imageNumber = match.replace(":e","").replace(":","");
    return imageNumber > 0 && imageNumber <= 200 ? `<img src="../assets/images/emojis/${imageNumber}.png"  data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="${match}" alt="${match}" class="emoji">` : match;
  });

  return text;
}

async function CreateMessage(message, isOld = false) {
  const msg = document.createElement("div");
  msg.classList.add("chat-message");
  msg.setAttribute("data-user", message.sender.username);
  msg.setAttribute("data-user-id", message.sender.id);
  msg.setAttribute("data-message", message.id);
  msg.innerHTML = MessageTemplate(message, isOld);
  document.getElementById("ChatBox").appendChild(msg);

  if (msg.querySelector(".delete")) {
    msg.querySelector(".delete").addEventListener("click", function (e) {
      var message_id = message.id;
      connection.invoke("DeleteMessage", message_id,GetRoom()).then((response) => {});
    });

    msg.querySelector(".timeout").addEventListener("click", function (e) {
      var userID = this.closest(".chat-message").getAttribute("data-user");
      connection.invoke("Timeout", GetRoom(),userID).then((response) => {
      });
    });

    msg.querySelector(".ban").addEventListener("click", function (e) {
      var userID = this.closest(".chat-message").getAttribute("data-user");
      connection.invoke("Ban", GetRoom(),userID).then((response) => {
      });
    });
    
    msg.querySelector(".pin-button").addEventListener("click", function (e) {
      var userID = this.closest(".chat-message").getAttribute("data-user");
      var message_id = message.id;
      connection.invoke("Pin", message_id,CurrentUser).then((response) => {
      });
    });
  }
}

function CreateMessageElement(message, isOld = false) {
  const msg = document.createElement("div");
  msg.classList.add("chat-message");
  msg.setAttribute("data-user", message.sender.username);
  msg.setAttribute("data-user-id", message.sender.id);
  msg.setAttribute("data-message", message.id);
  msg.innerHTML = MessageTemplate(message, isOld);
  return msg;
}

function GenerateUerBadges(roles,width = 18) {
  var badges = "";
  if (GetBadge(Badges.Broadcaster, roles)) {
    badges += `
        <span class="item-icons" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Broadcaster">
            <img src='../../assets/images/roles/broadcaster.png' width='${width}' height='${width}' />
        </span>
        `;
  }
  if (GetBadge(Badges.Moderator, roles)) {
    badges += `
        <span class="item-icons" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Moderator">
            <img src='../../assets/images/roles/moderator.png' width='${width}' height='${width}' />
        </span>
        `;
  }
  if (GetBadge(Badges.Vip, roles)) {
    badges += `
        <span class="item-icons" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="VIP">
            <img src='../../assets/images/roles/vip.png' width='${width}' height='${width}' />
        </span>
        `;
  }
  if (GetBadge(Badges.Prime, roles)) {
    badges += `
        <span class="item-icons" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Prime Gaming">
            <img src='../../assets/images/roles/prime.png' width='${width}' height='${width}' />
        </span>
        `;
  }
  if (GetBadge(Badges.Verified, roles)) {
    badges += `
        <span class="item-icons" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Verified">
            <img src='../../assets/images/roles/verified.png' width='${width}' height='${width}' />
        </span>
        `;
  }
  return badges;
}

function PreventMessage(prevent = true,logMessageInChat = true) {
  MessageInput.setAttribute(
    "placeholder",
    prevent
      // ? `You timed out, can chat after ${TimeOutSeconds} seconds.`
      ? `You timed out from Chat.`
      : "Send a message."
  );
  MessageInput.disabled = prevent;
  document.querySelector(".send-button").disabled = prevent;
  if (prevent) {
    MessageInput.value = "";
    document.querySelector(".message-box").classList.add("border-danger");
    // Timeout(TimeOutSeconds - 1);
    // Start Count Down
  } else {
    document.querySelector(".message-box").classList.remove("border-danger");
  }
  
  if (logMessageInChat) {
    const msg = document.createElement("div");
    msg.innerHTML = `<span class='welcome-text'>${
      prevent
        ? "You timed out for 15 seconds by the moderator."
        : "You can chat now."
    }</span>`;
    document.getElementById("ChatBox").appendChild(msg);
  }

  if (prevent) {
    MessageMarkup.removeAttribute("contenteditable");
  } else {
    MessageMarkup.setAttribute("contenteditable","true");
  }
  ChatBox.scrollTo({ top: ChatBox.scrollHeight, behavior: "smooth" });
}

//Tooltips
function setToolTips() {

  // Remove Active all Tips
  document.querySelectorAll(".tooltip").forEach(e => e.remove());

  const tooltipTriggerList = document.querySelectorAll(
    '[data-bs-toggle="tooltip"]'
  );
  const tooltipList = [...tooltipTriggerList].map(
    (tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl)
  );
}


function LoadEmojies() {
  FloatingBox.innerHTML = EmojiBoxTemaplte();
  FloatingBox.querySelector(".emoji-box-body").innerHTML = FillEmojies();
  FloatingBox.querySelector(".close-emoji-box").addEventListener("click",function(e){
    CloseFloatingBox();
  })
}


function FillEmojies() {
  var List = "";
  for (let i = 0; i < 200; i++) {
    List += `<img src="../assets/images/emojis/${i + 1}.png"  data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title=":e${i+1}:" alt=":e${i+1}:" class="emoji">`;
  }
  return List;
}

function CloseFloatingBox() {
  FloatingBox.innerHTML = "";
  FloatingBox.classList.add("d-none");
}

function CloseReplyMessage() {
  if (ReplyBox.querySelector(".close-reply-box")) {
      ReplyBox.querySelector(".close-reply-box").removeEventListener("click",CloseReplyMessage);
      ReplyBox.innerHTML = "";
      ReplyBox.closest(".message-outline-wrapper").classList.remove("active");
      ReplyMessageId = null;
      Root.style.setProperty("--controls-height","100px");
      MessageInput.setAttribute("placeholder", "Send a message.");
  }
}


function HideGUI(inputPlaceHolder) {
  MessageInput.value = '';
  MessageInput.setAttribute("placeholder", inputPlaceHolder);
  MessageInput.setAttribute("disabled", "disabled");
  MessageMarkup.removeAttribute("contenteditable");
  document.querySelector(".send-button").disabled = true;
  EmojiButton.remove();
  PinMsg.classList.add("d-none");
  RestorePinMsg.classList.add("d-none");
  ConnectionTab.classList.add("d-none");
  document.querySelector(".mod-icons-area").remove();
  document.querySelector(".settings-area").remove();
  document.querySelector(".message-left").remove();
  CommunityBtn.remove();
  Community.remove();

}