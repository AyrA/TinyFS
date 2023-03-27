# TinyFS

This is a library and tool to bundle multiple files together into a small file system.
It's intended for systems with severe size constraints,
and as such comes with various limitations.

If sizes are not a problem for you, using a zip file is likely the better option.

For documentation about the Gui,
head over to [TinyFS help](https://help.ayra.ch/tinyfs)

## Features

- Bundle multiple files together
- Case insensitive file name option
- Data compression option
- AES Encryption option

## Limitations

- Container holds at most 255 files
- File names can be at most 255 bytes long
- A single file can be at most 65535 bytes long
- the FS is not designed to be written to

## Usage

You can either add TinyFSLib to your project,
or use its basic features from the command line using the TinyFS application.

# File system format

The basic format is a header, followed by the index, followed by data.
Index and data can optionally be encrypted.

All data is stored in little endian byte order, and all numbers are unsigned.

## Header

| Value | Size | Description           |
|-------|------|-----------------------|
| Magic | 4    | `0x54 0x49 0x4E 0x59` |
| Flags | 1    | See below for details |

### Possible flag values

This lists the possible flags with their numerical values.
3 Flags are currently defined.
A TinyFS processor should abort if unknown flags are set.

#### 0x00: No special flags

There are no special flag set on this container.

#### 0x01: Case insensitive mode

If set, file names should be treated and compared in a case insensitive manner.

Applications that create TinyFS containers with this flag set
are not required to maintain the original name casing either,
and may convert all names to uppercase.

#### 0x02: Encryption

The TinyFS container is encrypted if this flag is set.
How encrypted content is stored is explained further down.

#### 0x04: UTF-8

If set, file names are UTF-8 encoded.
If this is not set, file names are treated to be 7-bit ASCII.
It's best to leave this flag unset if the file system is used on a microcontroller,
as unicode support on those is often lacking.

## Index

The index follows the header.

| Value  | Size | Description            |
|--------|------|------------------------|
| Count  | 1    | Number of FS entries   |
| Header | ??   | Repeated "Count" times |

A valid TinyFS file has at least one entry.
Entries are of different size depending on their content,
but individually are at most 259 bytes.

### Entry

An entry consists of four values in this order:

| Value       | Size | Description                     |
|-------------|------|---------------------------------|
| Flags       | 1    | Flags specific to this file     |
| Data size   | 2    | Length of file data             |
| Name length | 1    | Length in bytes, not characters |
| Name        | ??   | Size specified by "Name length" |

The name is not terminated in any way.
If your language requires string termination you must add it yourself,
and thus also reserve the extra memory required for it (in C, usually one extra byte)

The data length always specifies the number of bytes that must be read from the underlying stream.
This means that for compressed files, this number will be the compressed length.

The TinyFSLib.FileData instances always present the data in its real form,
that is with all encodings processed.
In simple terms this means the library accepts new data as-is and presents it identically again later.
It transparently does all encoding and compression in the background,
similar to how real file systems compress files.

The flags can consist of the values below

#### 0x00: No flags

Regular file entry

#### 0x01: Encryption

The data is stored encrypted.
Not currently supported.

#### 0x02: GZip

The data is stored in GZip format.
Note: File data is always compressed individually,
even if multiple index entries in a row specify GZip encoding.

## Data

The data is all file data bytes in the order of the index concatenated together.
To decode the data of the "nth" entry, the sizes of the preceeding entries must be summed up to get the data offset.

# Encryption

TinyFS files can be encrypted.
The standard chosen is AES-GCM, and encryption can either be done by directly giving an AES key of 32 bytes,
or by supplying a password, which will internally be fed through a PBKDF2 function.

Note: Encrypting the data will increase the size of the container.
The AES function itself adds 28 bytes.
If you use a password instead of a key, it adds an additional 16 bytes,
resulting in an increase of 44 bytes compared to an unencrypted container.
if you intend to pad the container to a certain size, you must take this into account.

## Changes to the container format

If the encryption flag is set on a container, it means everything after the flag is encrypted.
The flag is followed by an integer that specifies the number of bytes to read.
Those bytes contain the index and data portions of the container once decrypted.

To conserve size, the integer specifying the encrypted size is given using a 7-bit encoding scheme.
This means the size is stored 7 bits at a time in the lowest 7 bits of a byte.
The highest bit in the byte is used to indicate whether an additional byte has to be read for the full integer.
The integer is 32 bits, meaning up to at most 5 bytes will be needed to store the value,
however, due to the container size limitations (see further below), 4 bytes is guaranteed to be enough.

## Encryption format

The encrypted data is the concatenation of three or four values

| Value       | Size | Description                     |
|-------------|------|---------------------------------|
| Salt        | 16   | Salt for the password function  |
| Nonce       | 12   | AES-GCM nonce                   |
| Tag         | 16   | AES-GCM tag                     |
| Data        | ??   | AES-GCM encrypted data          |

Note: The salt is only present if the TinyFS container was encrypted using a password instead of a key.
There is however no way of detecting this,
and the user has to remember his choice from when he encrypted the file.

## Password based key derivation

PBKDF2 is chosen as the password based key derivation function.
The salt is a randomly generated 16 byte sequence that's added in front of the encrypted data blob
(see table above for details)

Other parameters in use but not stored

- Hashing function: SHA-256
- Iterations: 100'000
- Derived key size: 32 bytes

## Container Size

Due to the various size limitations, a TinyFS container can at most be this big:

- Unencrypted: 16'777'476 bytes (16.7 MB)
- Encrypted: 16'777'524 bytes (16.7 MB)

The size is given by having 255 maximum size index entries with 255 maximum size data entries
plus the basic TinyFS header of 6 bytes: $6 + 255 * 259 + 255 * 65535 = 16777476$
