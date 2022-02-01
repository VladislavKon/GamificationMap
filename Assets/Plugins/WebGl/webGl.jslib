mergeInto(LibraryManager.library, {
  GameOver: function (userName, score) {
    window.dispatchReactUnityEvent(
      "GameOver",
      Pointer_stringify(userName),
      score
    );
  },
});