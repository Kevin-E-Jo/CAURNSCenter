mergeInto( LibraryManager.library, {
    openLoadingModal: function (){
        window.dispatchReactUnityEvent(
   	    	    "openLoadingModal"
        );
    },
    closeLoadingModal: function (){
        window.dispatchReactUnityEvent(
         	    "closeLoadingModal"
        );
    },
    okayToLeave: function (){
        window.dispatchReactUnityEvent(
         	    "okayToLeave"
        );
    },
    pingAck: function (){
         window.dispatchReactUnityEvent(
          	    "pingAck"
         );
    },
    onUnityLoaded: function (){
         window.dispatchReactUnityEvent(
          	    "onUnityLoaded"
         );
    },
    onDisconnectServer: function (){
         window.dispatchReactUnityEvent(
                "onDisconnectServer"
         );
    },
    requestUIChange: function (type){
         window.dispatchReactUnityEvent(
              "requestUIChange",
              Pointer_stringify(type)
         );
    },
    requestWindow: function (type){
         window.dispatchReactUnityEvent(
              "requestWindow",
              Pointer_stringify(type)
         );
    },
    openLectureModal: function (){
         window.dispatchReactUnityEvent(
                "openLectureModal"
         );
    },
    setUIByType: function (type){
         window.dispatchReactUnityEvent(
              "setUIByType",
              Pointer_stringify(type)
         );
    },
    closeMiniMap: function (){
         window.dispatchReactUnityEvent(
                "closeMiniMap"
         );
    }
});