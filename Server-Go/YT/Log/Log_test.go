package Log

import (
	"github.com/sirupsen/logrus"
	"os"
	"testing"
)

func TestLog(t *testing.T) {
	log := logrus.New()
	file, err := os.OpenFile("Test.log", os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)
	if err == nil {
		log.Out = file
	} else {
		log.Info("Failed to log to file, using default stderr")
	}
	log.Info("姚姚姚")
}
