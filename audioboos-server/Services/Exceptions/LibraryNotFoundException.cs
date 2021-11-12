using System;

namespace AudioBoos.Server.Services.Exceptions;

public class LibraryNotFoundException : Exception {
    public LibraryNotFoundException(string message) : base(message) {
    }
}
