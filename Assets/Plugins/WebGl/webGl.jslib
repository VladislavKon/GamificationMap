mergeInto(LibraryManager.library, {
  GameOver: function (userName, score) {
    window.dispatchReactUnityEvent(
      "GameOver",
      Pointer_stringify(userName),
      score
    );
  },
  SaveGame: function (map) {
    window.dispatchReactUnityEvent(
      "SaveGame",
      Pointer_stringify(map)
    );
  }
});