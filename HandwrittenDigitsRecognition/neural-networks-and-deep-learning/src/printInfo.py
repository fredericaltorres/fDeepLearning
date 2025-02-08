import gzip
import pickle
import numpy as np


def save_mnist_labels(input_file="../data/mnist.pkl.gz", output_file="mnist_labels.txt"):
    try:
        # Load the MNIST dataset from the pickle file
        with gzip.open(input_file, 'rb') as f:
            # The MNIST pickle file contains three tuples: training, validation, and test sets
            train_set, valid_set, test_set = pickle.load(f, encoding='latin1')
            
            # Each set is a tuple of (data, labels)
            train_data, train_labels = train_set
            valid_data, valid_labels = valid_set
            test_data, test_labels = test_set
            
            # Combine all labels
            all_labels = np.concatenate((train_labels, valid_labels, test_labels))
            
            # Write labels to output file
            with open(output_file, 'w') as out_f:
                for label in all_labels:
                    out_f.write(f"{label}\n")
                    
        print(f"Successfully processed {len(all_labels)} labels")
        print(f"Dataset composition:")
        print(f"Training set: {len(train_labels)} images")
        print(f"Validation set: {len(valid_labels)} images")
        print(f"Test set: {len(test_labels)} images")
        print(f"Output written to: {output_file}")
        
        # Print distribution of digits
        unique, counts = np.unique(all_labels, return_counts=True)
        print("\nDigit distribution:")
        for digit, count in zip(unique, counts):
            print(f"Digit {digit}: {count} images")
            
    except FileNotFoundError:
        print(f"Error: Could not find input file {input_file}")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    save_mnist_labels()