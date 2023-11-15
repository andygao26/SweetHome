const chatInput = document.querySelector(".chat-input textarea");
const sendChatBtn = document.querySelector(".chat-input span");
const chatbox = document.querySelector(".chatbox");

let userMessage;
const API_KEY = "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";

// 定義createChatLi，接收message、className(CSS類別名稱)兩個參數
const createChatLi = (message, className) => {
    const chatLi = document.createElement("li");//創建一個 <li> 元素，代表一個聊天訊息。
    chatLi.classList.add("chat", className);//將這兩個 CSS 類別名稱添加到創建的 <li> 元素上。
    const chatContent = className === "outgoing" ? `<p>${message}</p>` : `<p>${message}</p>`;
    //建了一個變數 chatContent 來存放聊天訊息的 HTML 內容。根據 className 的不同，它會根據訊息是 "outgoing"（使用者傳送的訊息）還是其他情況來創建不同的 <p> 元素內容。
    chatLi.innerHTML = chatContent;// 將創建的 chatContent HTML 內容設定為 <li> 元素的內部 HTML。
    return chatLi;//返回創建的 <li> 元素，作為函數的結果。
};

//根據使用者傳入的訊息（userMessage）生成回覆內容並將其插入到聊天室中。
const generateResponse = (incomingChatLi) => {
    let responseContent;//用於存放生成的回應內容。

    switch (userMessage) {
        case "登入問題":
            break;
        case "關於登入帳號":
            responseContent = "登入帳號預設為棟別+樓層號+門牌號碼";
            break;
        case "忘記密碼":
            responseContent = "回應 BBB";
            break;
        case "忘記密碼":
            responseContent = "密碼預設為身分證字號";
            break;
        case "維修問題":
            responseContent = "您選擇了選項 2 的回答。";
            break;
        case "其他問題":
            responseContent = "您選擇了選項 3 的回答。";
            break;
        default:
            responseContent = "我不確定如何回答您的選擇。";
    }

    // 插入新的回應內容
    const responseElement = createChatLi(responseContent, "incoming");
    chatbox.appendChild(responseElement);

    chatbox.scrollTo(0, chatbox.scrollHeight);
};

const handleChat = () => {
    userMessage = chatInput.value.trim();
    if (!userMessage) return;

    // Append the user's message to the chatbox
    chatbox.appendChild(createChatLi(userMessage, "outgoing"));
    chatbox.scrollTo(0, chatbox.scrollHeight);

    // 生成回應並插入
    generateResponse();
};

sendChatBtn.addEventListener("click", handleChat);

const options = document.querySelectorAll(".option");

// 處理選擇按鈕的點擊事件
options.forEach(option => {
    option.addEventListener("click", () => {
        const selectedOption = option.textContent;
        handleUserChoice(selectedOption);
    });
});

// 處理使用者選擇
const handleUserChoice = (selectedOption) => {
    userMessage = selectedOption;

    // Append the user's message to the chatbox
    chatbox.appendChild(createChatLi(userMessage, "outgoing"));
    chatbox.scrollTo(0, chatbox.scrollHeight);

    if (selectedOption === "登入問題") {
        // 插入進階選項的 HTML 內容
        const advancedOptions = `
        <li class="chat incoming" id="loginOptions">
             <span class="material-symbols-outlined">smart_toy</span>    
            <div class="options">
                    <p>
                    <a href="#" class="option chat-link">關於登入帳號</a><br>
                    <a href="#" class="option chat-link">忘記密碼</a>
                </p>
            </div>
        </li>`;

        // 插入進階選項
        const advancedOptionsElement = document.createRange().createContextualFragment(advancedOptions);
        chatbox.appendChild(advancedOptionsElement);
        chatbox.scrollTo(0, chatbox.scrollHeight);

        // 更新 userMessage 為進階選項的內容
        userMessage = advancedOptions;
        
        // Append the user's message to the chatbox
        chatbox.appendChild(createChatLi(userMessage, "outgoing"));
        chatbox.scrollTo(0, chatbox.scrollHeight);

    } else if (selectedOption === "關於登入帳號") {
        // 生成回應並插入
        generateResponse("登入帳號預設為棟別+樓層號+門牌號碼");

        // 更新 userMessage 為 "關於登入帳號"
        userMessage = "關於登入帳號";
    } else if (selectedOption === "忘記密碼") {
        // 生成回應並插入
        generateResponse("密碼預設為身分證字號");

        // 更新 userMessage 為 "忘記密碼"
        userMessage = "忘記密碼";
    } else {
        // 若不是 "登入問題"，則生成回應並插入
        generateResponse();
    }
};
