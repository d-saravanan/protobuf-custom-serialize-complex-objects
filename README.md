# protobuf-custom-serialize-complex-objects
This repository contains the code sample that demonstrates how to create DTO's to manage the complex BCL Type serialization and thus reducing the amount of data in case of a very complex object.

## credits
Thanks to Marc Gravell for helping me resolve a related issue with the custom serialization of complex objects. More Insights and our discussions [here](https://github.com/mgravell/protobuf-net/issues/257)

## Details
In this application, we have a problem requiring to serialize a complex .Net Framework Type [ClaimsPrincipal] and for which we are using a custom object like a DTO to use only a few or critical properties for serialization. This sample helped me learn and know about protobuf-net and also explore on the custom serialization. Furthermore, learnt "implicit operator" functionality too.

In case of any one exploring on similar options, this can be a good starting point and if any other details are required, please post the question in the issues section.
