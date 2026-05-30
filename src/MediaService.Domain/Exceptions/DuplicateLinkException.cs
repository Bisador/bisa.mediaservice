namespace MediaService.Domain.Exceptions;

public class DuplicateLinkException() : Exception("Link is Duplicated.");

public class MediaIsNotAvailableException() : Exception("Media is not available.");

public class AttachmentMustHaveOnlyOneLinkException() : Exception("Attachment must have only one link.");

public class AttachmentLinkCantBeRemovedException() : Exception("Attachment link can't be removed.");

public class MediaWithLinksCantBeRemovedException() : Exception("Media with links can't be removed.");