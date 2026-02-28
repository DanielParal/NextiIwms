# Use the official RabbitMQ image with the management plugin
FROM rabbitmq:4.0-management

# Switch to root user to install packages
USER root

# Install tzdata and set timezone
RUN apt-get update && apt-get install -y tzdata

# Set default timezone if not set
ENV TZ=Europe/Prague

# ===========
# PLUGIN SECTION - currently we cannot run plugin in all environments. We need to sort it out.
# ===========

# Add the RabbitMQ Delayed Message Exchange plugin
# ADD https://github.com/rabbitmq/rabbitmq-delayed-message-exchange/releases/download/v4.0.2/rabbitmq_delayed_message_exchange-4.0.2.ez /opt/rabbitmq/plugins/

# Add rights to the file
# RUN chown rabbitmq:rabbitmq /opt/rabbitmq/plugins/rabbitmq_delayed_message_exchange-4.0.2.ez

# Enable the delayed message exchange plugin
# RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange