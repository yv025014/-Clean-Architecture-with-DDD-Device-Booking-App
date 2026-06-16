namespace DBA.Application.Common.Exceptions;

public class NotFoundException(Guid id) : Exception($"{id} ： 不存在");

