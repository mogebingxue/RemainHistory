package Log

import (
	"github.com/sirupsen/logrus"
	"os"
)

func NewLog(path string) *logrus.Logger {
	log := logrus.New()
	file, err := os.OpenFile(path, os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0666)
	if err == nil {
		log.Out = file
	} else {
		log.Info("Failed to log to file, using default stderr")
	}
	return log
}
