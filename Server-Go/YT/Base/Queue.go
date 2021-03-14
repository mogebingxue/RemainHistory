package Base

import "sync"

// minQueueLen is smallest capacity that queue may have.
// Must be power of 2 for bitwise modulus: x % n == x & (n - 1).
const minQueueLen = 16 // 队列缓存区最小长度

// Queue represents a single instance of the queue data structure.
type Queue struct {
	buf               []interface{} // 缓存区
	head, tail, count int           // 队头下标，队尾下标，队列长度
	lock              sync.Mutex
}

// New constructs and returns a new Queue.
func NewQueue() *Queue {
	return &Queue{
		buf: make([]interface{}, minQueueLen), // 返回最小长度缓存的空队列
	}
}

// Length returns the number of elements currently stored in the queue.
func (q *Queue) Length() int {
	return q.count // 队列长度
}

// resizes the queue to fit exactly twice its current contents
// this can result in shrinking if the queue is less than half-full
func (q *Queue) resize() {
	newBuf := make([]interface{}, q.count<<1) // 将队列缓存区长度重设为队列长度的两倍

	// 复制元素到缓存区开始区域
	if q.tail > q.head {
		copy(newBuf, q.buf[q.head:q.tail])
	} else {
		// 因为是环形缓存区，需要先复制缓存区后半部分，即队列前半部分，再复制缓存区前半部分，即队尾部分
		n := copy(newBuf, q.buf[q.head:])
		copy(newBuf[n:], q.buf[:q.tail])
	}

	q.head = 0       // 队头下标置0
	q.tail = q.count // 队尾下标为队列长度（此下标为下一个元素插入的下标）
	q.buf = newBuf   // 新队列
}

// Add puts an element on the end of the queue.
func (q *Queue) Enqueue(elem interface{}) {
	q.lock.Lock()
	defer q.lock.Unlock()
	if q.count == len(q.buf) {
		q.resize() // 新元素入队之前，当队列长度等于缓存区长度时，缓存区长度重设为两个队列长度
	}

	q.buf[q.tail] = elem // 入队
	// bitwise modulus
	q.tail = (q.tail + 1) & (len(q.buf) - 1) // 队尾下标在缓存环中移动一位
	q.count++                                // 队列长度+1

}

// Remove removes and returns the element from the front of the queue. If the
// queue is empty, return nil.
func (q *Queue) Dequeue() interface{} {
	q.lock.Lock()
	defer q.lock.Unlock()
	if q.count <= 0 {
		return nil
	}
	ret := q.buf[q.head] // 出队
	q.buf[q.head] = nil
	// bitwise modulus
	q.head = (q.head + 1) & (len(q.buf) - 1) // 队头下标在环形缓存区后移一位
	q.count--                                // 队列长度减一
	// Resize down if buffer 1/4 full.
	if len(q.buf) > minQueueLen && (q.count<<2) == len(q.buf) {
		q.resize() // 重置缓存区大小，当队列长度减小到缓存区长度1/4的时候，但不可小于最小长度16
	}
	return ret
}
