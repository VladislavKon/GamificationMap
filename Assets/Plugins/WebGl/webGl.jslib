mergeInto(LibraryManager.library, {
  GameOver: function (userName, score) {
    window.dispatchReactUnityEvent(
      "GameOver",
      Pointer_stringify(userName),
      score
    );
  },
  SaveMap: function (map) {
    window.dispatchReactUnityEvent(
      "SaveMap",
      Pointer_stringify(map)
    );
  },
  LoadMap: function (map) {
    window.dispatchReactUnityEvent(
      "LoadMap"
    );
  }
});