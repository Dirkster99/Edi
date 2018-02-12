// Authored by: John Stewien
// Year: 2011
// Company: Swordfish Computing
// License: 
// The Code Project Open License http://www.codeproject.com/info/cpol10.aspx
// Originally published at:
// http://www.codeproject.com/Articles/208361/Concurrent-Observable-Collection-Dictionary-and-So
// Last Revised: September 2012

namespace FsCore.Collections
{
    /// <summary>
    /// This class is used for linked objects in a 2 way linked list,
    /// which are also all held in a dictionary. The purpose of this
    /// class is to allow the position of items in a dictionary to be
    /// quickly determined.
    /// </summary>
    public class DoubleLinkListIndexNode {

    // ************************************************************************
    // Public Fields
    // ************************************************************************
    #region Public Fields

    /// <summary>
    /// The previous node in the linked list
    /// </summary>
    public DoubleLinkListIndexNode Previous;
    /// <summary>
    /// The next node in the linked list
    /// </summary>
    public DoubleLinkListIndexNode Next;
    /// <summary>
    /// The position within the linked list
    /// </summary>
    public int Index;

    #endregion Public Fields

    // ************************************************************************
    // Public Methods
    // ************************************************************************
    #region Public Methods

    /// <summary>
    /// Constructor for when a node is added to the end of the list.
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="index"></param>
    public DoubleLinkListIndexNode(DoubleLinkListIndexNode previous, int index) {
      Previous = previous;
      Next = null;
      Index = index;
      if (Previous != null) {
        Previous.Next = this;
      }
    }

    /// <summary>
    /// Constructor for when a node is inserted into the middle of the list.
    /// </summary>
    /// <param name="previous"></param>
    /// <param name="index"></param>
    public DoubleLinkListIndexNode(DoubleLinkListIndexNode previous, DoubleLinkListIndexNode next) {
      Previous = previous;
      Next = next;
      Index = next.Index;
      if (Previous != null) {
        Previous.Next = this;
      }
      if (Next != null) {
        Next.Previous = this;
        IncrementForward();
      }
    }

    /// <summary>
    /// This function effectively removes this node from the linked list,
    /// and decrements the position index of all the nodes that follow it.
    /// It removes the node by changing the nodes that come before and
    /// after it to point to each other, thus bypassing this node.
    /// </summary>
    public void Remove() {
      if (Previous != null) {
        Previous.Next = Next;
      }
      if (Next != null) {
        Next.Previous = Previous;
      }
      DecrementForward();
    }

    #endregion Public Methods

    // ************************************************************************
    // Private Methods
    // ************************************************************************
    #region Private Methods

    /// <summary>
    /// This recursive function decrements the position index of all the nodes
    /// in front of this node. Used for when a node is removed from a list.
    /// </summary>
    private void DecrementForward() {
      if (Next != null) {
        Next.Index--;
        Next.DecrementForward();
      }
    }

    /// <summary>
    /// This recursive function decrements the position index of all the nodes
    /// in front of this node. Used for when a node is inserted into a list.
    /// </summary>
    private void IncrementForward() {
      if (Next != null) {
        Next.Index++;
        Next.IncrementForward();
      }
    }

    #endregion Private Methods
  }
}
