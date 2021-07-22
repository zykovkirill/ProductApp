window.P00340 = {
    start: (token) =>  {

        var buildUrl = "unity/P00340/web/Build";
    var loaderUrl = buildUrl + "/web.loader.js";
      var config = {
        dataUrl: buildUrl + "/web.data",
        frameworkUrl: buildUrl + "/web.framework.js",
        codeUrl: buildUrl + "/web.wasm",
        streamingAssetsUrl: "StreamingAssets",
        companyName: "DefaultCompany",
        productName: "New Unity Project (5)",
        productVersion: "1.0",
      };

      var container = document.querySelector("#unity-container");
      var canvas = document.querySelector("#unity-canvas");
      var loadingBar = document.querySelector("#unity-loading-bar");
      var progressBarFull = document.querySelector("#unity-progress-bar-full");
      var fullscreenButton = document.querySelector("#unity-fullscreen-button");
      var mobileWarning = document.querySelector("#unity-mobile-warning");

      if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
        container.className = "unity-mobile";
        config.devicePixelRatio = 1;
        mobileWarning.style.display = "block";
        setTimeout(() => {
        mobileWarning.style.display = "none";
        }, 5000);
      } else {
        canvas.style.width = "960px";
        canvas.style.height = "600px";
      }
      loadingBar.style.display = "block";

      var script = document.createElement("script");
      script.src = loaderUrl;
      script.onload = () => {
        createUnityInstance(canvas, config, (progress) => {
            progressBarFull.style.width = 100 * progress + "%";
        }).then((unityInstance) => {
            unityInstance.SendMessage('Constants', 'SetToken', token);//!!!!
            loadingBar.style.display = "none";
            fullscreenButton.onclick = () => {
                unityInstance.SetFullscreen(1);
            };
        }).catch((message) => {
            alert(message);
        });
      };
      document.body.appendChild(script);
    }
}