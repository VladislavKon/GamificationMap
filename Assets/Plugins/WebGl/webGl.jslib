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
  UpdateCell: function (cell) {
    window.dispatchReactUnityEvent(
      "UpdateCell",
      Pointer_stringify(cell)
    );
  },
  LoadMap: function () {
    window.dispatchReactUnityEvent(
      "LoadMap"
    );
  },
  GetCurrentUser: function () {
    window.dispatchReactUnityEvent(
      "GetCurrentUser"
    );
  },
  GetAllTeams: function () {
    window.dispatchReactUnityEvent(
      "GetAllTeams"
    );
  },
  SubtractPoints: function (CurrentPlayer, points) {
    window.dispatchReactUnityEvent(
      "SubtractPoints",
      Pointer_stringify(CurrentPlayer, points)
    );
  }
});