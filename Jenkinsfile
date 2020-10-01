properties([pipelineTriggers([githubPush()])])

pipeline {
    agent { dockerfile true }
    environment {
        SONARQUBE_PROJECT_KEY = "vtb-templatesservice"
        SONARQUBE_TOKEN = credentials('SONARQUBE_TOKEN');
        SONARQUBE_URL = credentials('SONARQUBE_URL');

        BAGET_USERNAME = credentials('BAGET_USERNAME');
        BAGET_PASSWORD = credentials('BAGET_PASSWORD');
        BAGET_HOST = credentials('BAGET_HOST');
        BAGET_API_KEY = credentials('BAGET_API_KEY');

        BRANCH_NAME = "${env.BRANCH_NAME}";
        VERSION = "1.0.${BUILD_NUMBER}"
    }

    stages {
        stage('SCA Setup') {
            steps {
                sh '''\
                    dotnet sonarscanner begin \
                        /k:"${SONARQUBE_PROJECT_KEY}" \
                        /d:sonar.login="${SONARQUBE_TOKEN}" \
                        /d:sonar.host.url="${SONARQUBE_URL}" \
                        /d:sonar.coverage.exclusions="*.Tests/" \
                        /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
                    '''
            }
        }
        stage('Restore') {
            steps {
                sh 'dotnet nuget add source https://${BAGET_HOST}/v3/index.json --username ${BAGET_USERNAME} --password ${BAGET_PASSWORD} --store-password-in-clear-text --name BaGet'
                sh 'dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -p:Version=${VERSION} -c Release --no-restore'
            }
        }
        stage('Run All Tests') {
            // steps {
            //     sh '''\
            //         dotnet test \
            //             -c Release \
            //             --no-build \
            //             --logger "trx" \
            //             /p:CollectCoverage=true \
            //             /p:CoverletOutputFormat="opencover" \
            //             /p:CoverletOutput=./
            //     '''
            //     mstest()
            // }            
            steps {
                sh '''\
                    dotnet test \
                        -c Release \
                        --no-build \
                        --logger "trx" \
                        /p:CollectCoverage=true \
                        /p:CoverletOutputFormat="opencover" \
                        /p:CoverletOutput=./
                '''
                mstest()
            }
        }
        stage('SCA Upload') {
            steps {
                sh 'dotnet sonarscanner end /d:sonar.login="${SONARQUBE_TOKEN}"'
            }
        }
    }

    post {
        cleanup {
            echo "Cleaning up"
            sh 'rm -rfv ./*'
            sh 'dotnet nuget remove source BaGet'
        }
    }
}